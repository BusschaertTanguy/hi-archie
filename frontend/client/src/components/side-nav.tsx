import { Link } from "@tanstack/react-router";

const SideNav = () => {
  return (
    <nav className="flex w-60 flex-col gap-6 p-6">
      <Link to="/communities" search={{ pageIndex: 0, pageSize: 25 }}>
        Communities
      </Link>
    </nav>
  );
};

export default SideNav;
