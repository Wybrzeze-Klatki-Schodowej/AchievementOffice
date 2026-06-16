import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./Navbar.css";
import { logout } from "../../api/LoginApi";
import { getCurrentUser } from "../../api/LoginApi";
import NotificationBell from "../notifications/NotificationBell";

interface Props {
    onAddAchievementClick?: () => void;
    onAddShoutoutClick?: () => void;
    notificationsRefreshTrigger: number;
}

interface User {
    userId: string;
    login: string;
    role: string;
}

export default function Navbar({
    onAddAchievementClick,
    onAddShoutoutClick,
    notificationsRefreshTrigger
}: Props) {
    const navigate = useNavigate();

    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        getCurrentUser()
            .then((userData) => {
                setUser(userData);
                setLoading(false);
            })
            .catch((error) => {
                console.error("Error fetching user in navbar:", error);
                setLoading(false);
            });
    }, []);

    const handleProfileClick = () => {
        if (user?.userId) {
            navigate(`/users/${user.userId}`);
        }
    };

    const handleAdminClick = () => {
        if (user?.userId) {
            navigate(`/admin/users`);
        }
    };

    return (
        <nav className="navbar">
            <div className="navbar-left">
                <h2>Achievement Office</h2>
            </div>

            <div className="navbar-right">
                <NotificationBell 
                    refreshTrigger={notificationsRefreshTrigger}
                />

                {!loading && user?.role === "Admin" && (
                    <button
                        onClick={handleAdminClick}
                    >
                        Admin dashboard
                    </button>
                )}

                <button onClick={() => navigate('/groups')}>
                    Groups
                </button>

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