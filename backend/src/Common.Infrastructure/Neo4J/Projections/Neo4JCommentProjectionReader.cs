using Core.Application.Comments.Projections;
using Neo4j.Driver;

namespace Common.Infrastructure.Neo4j.Projections;

internal sealed class Neo4JCommentProjectionReader(IDriver driver) : ICommentProjectionReader
{
    public async Task<List<CommentProjection>> GetCommentsAsync(Guid postId)
    {
        await using var session = driver.AsyncSession();

        var records = await session.ExecuteReadAsync(async tx =>
        {
            var response = await tx.RunAsync(
                "MATCH path=(post:POST {id: $id})<-[:IS_COMMENT_OF*1..]-() " +
                "WITH nodes(path) as nodes " +
                "WITH nodes[size(nodes)-2] AS parent, nodes[size(nodes)-1] AS child " +
                "RETURN child.id, parent.id, child.up, child.down, child.content, child.publishDate, child.ownerId " +
                "ORDER BY (child.up - child.down) DESC", new { id = postId.ToString() });

            return await response.ToListAsync();
        });

        var lookup = new Dictionary<Guid, CommentProjection>();
        var rootNodes = new List<CommentProjection>();

        foreach (var record in records)
        {
            var comment = new CommentProjection
            {
                Id = Guid.Parse(record[0].As<string>()),
                ParentId = Guid.Parse(record[1].As<string>()),
                Up = record[2].As<long>(),
                Down = record[3].As<long>(),
                Content = record[4].As<string>(),
                PublishDate = record[5].As<DateTimeOffset>(),
                OwnerId = Guid.Parse(record[6].As<string>()),
                Comments = []
            };

            lookup[comment.Id] = comment;
        }

        foreach (var comment in lookup.Values)
        {
            if (comment.ParentId == postId)
            {
                rootNodes.Add(comment);
            }
            else
            {
                if (lookup.TryGetValue(comment.ParentId, out var value))
                {
                    value.Comments.Add(comment);
                }
            }
        }

        return rootNodes;
    }
}