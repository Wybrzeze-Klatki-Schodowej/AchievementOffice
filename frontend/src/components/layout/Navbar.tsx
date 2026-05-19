import { Link } from "react-router-dom";
import "./Navbar.css";
import { logout } from "../../api/LoginApi";

interface Props {
    onAddAchievementClick?: () => void;
}

export default function Navbar({
    onAddAchievementClick,
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

                <button className="logout-button" onClick={() => logout()}>Logout</button>
            </div>
        </nav>
    );
}