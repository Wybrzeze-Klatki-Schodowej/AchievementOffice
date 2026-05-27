import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./Navbar.css";
import { logout } from "../../api/LoginApi";
import { getCurrentUser } from "../../api/LoginApi";

interface Props {
    onAddAchievementClick?: () => void;
}

interface User {
    userId: string;
    login: string;
    role: string;
}

export default function Navbar({
    onAddAchievementClick,
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
            navigate(`/admin/users/${user.userId}`);
        }
    };

    return (
        <nav className="navbar">
            <div className="navbar-left">
                <h2>Achievement Office</h2>
            </div>

            <div className="navbar-right">
                {!loading && user?.role === "Admin" && (
                    <button
                        onClick={handleAdminClick}
                    >
                        Admin dashboard
                    </button>
                )}

                <button
                    onClick={handleProfileClick}
                >
                    Profile
                </button>

                <button onClick={onAddAchievementClick}>
                    Add achievement
                </button>

                <button className="logout-button" onClick={() => logout()}>Logout</button>
            </div>
        </nav>
    );
}