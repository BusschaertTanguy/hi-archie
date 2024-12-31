import { memo } from "react";
import { ArrowDownIcon, ArrowUpIcon } from "@heroicons/react/24/outline";
import useUser from "../../../../hooks/use-user.ts";
import { twMerge } from "tailwind-merge";
import {
  CommentsEnumsCommentVoteType,
  PostsEnumsPostVoteType,
} from "../../../../api/types";

export interface VoterProps {
  readonly votes: number;
  readonly onUp: () => void;
  readonly onDown: () => void;
  readonly selectedType?:
    | PostsEnumsPostVoteType
    | CommentsEnumsCommentVoteType
    | null;
}

const Voter = ({ votes, onUp, onDown, selectedType }: VoterProps) => {
  const { userId } = useUser();

  const iconClassName = twMerge("size-3");

  let upClassName = userId
    ? twMerge(iconClassName, "hover:cursor-pointer hover:text-blue-500")
    : iconClassName;

  if (
    selectedType === PostsEnumsPostVoteType.Upvote ||
    selectedType === CommentsEnumsCommentVoteType.Upvote
  ) {
    upClassName = twMerge(upClassName, "text-blue-500");
  }

  let downClassName = userId
    ? twMerge(iconClassName, "hover:cursor-pointer hover:text-red-500")
    : iconClassName;

  if (
    selectedType === PostsEnumsPostVoteType.Downvote ||
    selectedType === CommentsEnumsCommentVoteType.Downvote
  ) {
    downClassName = twMerge(downClassName, "text-red-500");
  }

  return (
    <div className="flex items-center gap-1.5">
      <div>
        <ArrowUpIcon
          className={upClassName}
          onClick={() => {
            if (userId) {
              onUp();
            }
          }}
        />
      </div>
      <div className="text-xs">{votes}</div>
      <div>
        <ArrowDownIcon
          className={downClassName}
          onClick={() => {
            if (userId) {
              onDown();
            }
          }}
        />
      </div>
    </div>
  );
};

export default memo(Voter);
