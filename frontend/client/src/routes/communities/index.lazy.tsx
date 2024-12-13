import { createLazyFileRoute, Link } from "@tanstack/react-router";
import { useGetApiV1Communities } from "../../api/types";
import { PlusIcon } from "@heroicons/react/24/solid";
import Modal from "../../components/modal.tsx";
import AddCommunity from "./-components/add-community.tsx";
import useDisclosure from "../../hooks/use-disclosure.ts";
import useDebounce from "../../hooks/use-debounce.ts";
import Button from "../../components/button.tsx";

export const Route = createLazyFileRoute("/communities/")({
  component: Communities,
});

function Communities() {
  const navigate = Route.useNavigate();
  const { pageIndex, pageSize, name } = Route.useSearch();
  const debouncedName = useDebounce(name, 500);

  const {
    isOpen,
    handlers: { open, close },
  } = useDisclosure();

  const { data, refetch } = useGetApiV1Communities({
    pageIndex,
    pageSize,
    name: debouncedName,
  });

  return (
    <>
      <div className="flex h-full divide-x">
        <div className="flex w-48 flex-col items-center p-6">
          <Button
            color="black"
            variant="filled"
            className="flex items-center justify-center gap-0.5"
            onClick={open}
          >
            <PlusIcon className="size-6" />
            CREATE
          </Button>
        </div>
        <div className="flex flex-1 flex-col gap-4 p-6">
          <div className="flex justify-between">
            <input
              className="rounded px-2 py-1 outline outline-1"
              placeholder="Search"
              value={name ?? ""}
              onChange={async (e) => {
                await navigate({
                  from: "/communities",
                  search: (prev) => ({ ...prev, name: e.currentTarget.value }),
                });
              }}
            />
            <div className="flex gap-4">
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
          <div className="flex flex-col gap-4">
            {data?.communities.map((c) => (
              <div
                key={c.id}
                className="rounded p-4 outline outline-1 outline-black"
              >
                {c.name}
              </div>
            ))}
          </div>
        </div>
      </div>
      {isOpen && (
        <Modal onClose={close}>
          <AddCommunity
            onCancel={close}
            onSave={async () => {
              close();
              await refetch();
            }}
          />
        </Modal>
      )}
    </>
  );
}
