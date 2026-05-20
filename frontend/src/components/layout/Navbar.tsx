import { Link } from "react-router-dom";
import "./Navbar.css";
import { logout } from "../../api/LoginApi";

interface Props {
    onAddAchievementClick?: () => void;
    onAddShoutoutClick?: () => void;
}

export default function Navbar({
    onAddAchievementClick,
    onAddShoutoutClick,
}: Props) {
    return (
        <nav className="navbar">
            <div className="navbar-left">
                <h2>Achievement Office</h2>
            </div>

            <div className="navbar-right">
                <Link to="/">
                    <button>Profile</button>
                </Link>

                <button onClick={onAddAchievementClick}>
                    Add achievement
                </button>

                <button onClick={onAddShoutoutClick}>
                    Add shout-out
                </button>

                <button className="logout-button" onClick={() => logout()}>Logout</button>
            </div>
        </nav>
    );
}