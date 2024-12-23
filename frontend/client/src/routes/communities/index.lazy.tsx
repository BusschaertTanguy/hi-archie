import { createLazyFileRoute, Link } from "@tanstack/react-router";
import { useGetApiV1Communities } from "../../api/types";
import Button from "../../components/button.tsx";
import useUser from "../../hooks/use-user.ts";

const Communities = () => {
  const { userId } = useUser();
  const navigate = Route.useNavigate();
  const { pageIndex, pageSize, search } = Route.useSearch();

  const { data } = useGetApiV1Communities({
    pageIndex,
    pageSize,
    name: search,
  });

  return (
    <>
      <div className="flex flex-1 flex-col gap-5">
        <div className="flex justify-between">
          <div className="text-2xl">Communities</div>
          {userId && (
            <Button
              color="black"
              variant="filled"
              onClick={() => navigate({ to: "/communities/add" })}
            >
              ADD
            </Button>
          )}
        </div>
        <div className="flex flex-col gap-4">
          {data?.communities.map((c) => (
            <div
              key={c.id}
              className="flex items-center justify-between rounded p-4 outline outline-1 outline-black hover:cursor-pointer"
              onClick={async () => {
                await navigate({
                  to: "/communities/$communityId",
                  params: { communityId: c.id },
                  search: { pageIndex: 0, pageSize: 25 },
                });
              }}
            >
              {c.name}
            </div>
          ))}
        </div>
        <div className="flex items-center justify-end gap-4">
          <Link
            className="rounded px-3 py-1 text-black outline outline-1 outline-black"
            from="/communities"
            to="."
            search={(prev) => ({ ...prev, pageIndex: prev.pageIndex - 1 })}
            disabled={pageIndex === 0}
          >
            PREVIOUS
          </Link>
          <Link
            className="rounded px-3 py-1 text-black outline outline-1 outline-black"
            from="/communities"
            to="."
            search={(prev) => ({ ...prev, pageIndex: prev.pageIndex + 1 })}
            disabled={(pageIndex + 1) * pageSize >= (data?.total ?? 0)}
          >
            NEXT
          </Link>
        </div>
      </div>
    </>
  );
};

export const Route = createLazyFileRoute("/communities/")({
  component: Communities,
});
