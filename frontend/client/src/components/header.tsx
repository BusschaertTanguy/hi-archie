import { useAuth0 } from "@auth0/auth0-react";
import Button from "./button.tsx";
import { useNavigate, useSearch } from "@tanstack/react-router";
import { memo, useEffect, useState } from "react";
import useDebounce from "../hooks/use-debounce.ts";

const Header = () => {
  const { loginWithRedirect, logout, isAuthenticated } = useAuth0();
  const { search } = useSearch({ from: "__root__" });
  const navigate = useNavigate();

  const [searchValue, setSearchValue] = useState<string | undefined>(search);
  const debouncedSearch = useDebounce(searchValue, 500);

  useEffect(() => {
    void navigate({
      to: ".",
      search: (prev) => ({
        ...prev,
        search: debouncedSearch,
      }),
    });
  }, [debouncedSearch, navigate]);

  return (
    <header className="flex justify-between p-6">
      <span>Hi Archie!</span>
      <input
        className="w-96 rounded px-2 py-1 outline outline-1"
        placeholder="Search"
        value={searchValue ?? ""}
        onChange={(e) => {
          setSearchValue(e.currentTarget.value || undefined);
        }}
      />
      {isAuthenticated ? (
        <Button
          variant="outlined"
          color="black"
          onClick={() =>
            logout({ logoutParams: { returnTo: window.location.origin } })
          }
        >
          Log out
        </Button>
      ) : (
        <Button
          variant="outlined"
          color="black"
          onClick={() => loginWithRedirect()}
        >
          Log in
        </Button>
      )}
    </header>
  );
};

export default memo(Header);
