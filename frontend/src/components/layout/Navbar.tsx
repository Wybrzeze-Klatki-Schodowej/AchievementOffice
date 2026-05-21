import { useNavigate } from "react-router-dom";
import "./Navbar.css";
import { logout } from "../../api/LoginApi";
import { getCurrentUser } from "../../api/LoginApi";

interface Props {
    onAddAchievementClick?: () => void;
    onAddShoutoutClick?: () => void;
}

export default function Navbar({
    onAddAchievementClick,
    onAddShoutoutClick,
}: Props) {
    const navigate = useNavigate();

    const handleProfileClick = async () => {
        try {
            const user = await getCurrentUser();

            navigate(
                `/users/${user.userId}`
            );
        } catch (error) {
            console.error("Error fetching user profile:", error);
        }
    };

    return (
        <nav className="navbar">
            <div className="navbar-left">
                <h2>Achievement Office</h2>
            </div>

            <div className="navbar-right">
                <button
                    onClick={handleProfileClick}
                >
                    Profile
                </button>

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