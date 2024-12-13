import { useAuth0 } from "@auth0/auth0-react";
import Button from "./button.tsx";

const Header = () => {
  const { loginWithRedirect, logout, isAuthenticated } = useAuth0();

  return (
    <header className="flex justify-between p-6">
      <span>Hi Archie!</span>
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

export default Header;
