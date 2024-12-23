import { createLazyFileRoute, Link, useNavigate } from "@tanstack/react-router";
import {
  CommunitiesQueriesGetCommunitiesDto,
  useGetApiV1Communities,
} from "../../api/types";
import { PlusIcon } from "@heroicons/react/24/solid";
import Modal from "../../components/modal.tsx";
import useDisclosure from "../../hooks/use-disclosure.ts";
import Button from "../../components/button.tsx";
import { useCallback, useState } from "react";
import CommunityForm from "./-components/community-form.tsx";
import useUser from "../../hooks/use-user.ts";

export const Route = createLazyFileRoute("/communities/")({
  component: Communities,
});

function Communities() {
  const { userId } = useUser();
  const navigate = useNavigate({ from: "/communities" });
  const { pageIndex, pageSize, search } = Route.useSearch();

  const [selectedCommunity, setSelectedCommunity] =
    useState<CommunitiesQueriesGetCommunitiesDto>();

  const {
    isOpen,
    handlers: { open, close },
  } = useDisclosure();

  const { data, refetch } = useGetApiV1Communities({
    pageIndex,
    pageSize,
    name: search,
  });

  const closeForm = useCallback(() => {
    close();
    setSelectedCommunity(undefined);
  }, [close]);

  return (
    <>
      <div className="flex flex-1 flex-col gap-5">
        <div className={`flex ${userId ? "justify-between" : "justify-end"}`}>
          {userId && (
            <Button
              color="black"
              variant="filled"
              className="flex items-center justify-center gap-0.5"
              onClick={open}
            >
              <PlusIcon className="size-6" />
              CREATE
            </Button>
          )}
        </div>
        <div className="flex flex-col gap-4">
          {data?.communities.map((c) => (
            <div
              key={c.id}
              className="flex items-center justify-between rounded p-4 outline outline-1 outline-black"
            >
              <div
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
              {userId === c.ownerId && (
                <Button
                  color={"black"}
                  variant={"outlined"}
                  onClick={() => {
                    setSelectedCommunity(c);
                    open();
                  }}
                >
                  EDIT
                </Button>
              )}
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
      {isOpen && (
        <Modal onClose={closeForm}>
          <CommunityForm
            community={selectedCommunity}
            onCancel={closeForm}
            onSave={async () => {
              closeForm();
              await refetch();
            }}
          />
        </Modal>
      )}
    </>
  );
}
