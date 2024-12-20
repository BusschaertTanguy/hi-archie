import { Link } from "@tanstack/react-router";
import { twMerge } from "tailwind-merge";
import { BookOpenIcon } from "@heroicons/react/24/solid";

const SideNav = () => {
  const linkClassName = twMerge("flex items-center gap-2 rounded px-3 py-1");
  const inactiveClassName = twMerge("border border-black", linkClassName);
  const activeClassName = twMerge("bg-black text-white", linkClassName);

  return (
    <nav className="flex w-64 flex-col gap-5 p-6">
      <Link
        to="/communities"
        search={{ pageIndex: 0, pageSize: 25 }}
        activeOptions={{
          exact: false,
          includeSearch: false,
        }}
        activeProps={{ className: activeClassName }}
        inactiveProps={{ className: inactiveClassName }}
      >
        <BookOpenIcon className="size-5" />
        Communities
      </Link>
    </nav>
  );
};

export default SideNav;
