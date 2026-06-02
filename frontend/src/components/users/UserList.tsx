import { useEffect, useState } from "react";
import { getAllUsers } from "../../api/UserApi";
import { useNavigate } from "react-router-dom";

interface UserListItem {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
    isActive: boolean;
}

interface Props {
    refreshTrigger: number;
}

export default function UserList({
    refreshTrigger
}: Props) {
    const [users, setUsers] = useState<UserListItem[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        getAllUsers()
            .then(setUsers)
            .catch(err => {
                console.error("Error fetching users:", err);
                alert("Failed to load users. Please try again later.");
            });
    }, [refreshTrigger]);

    return (
        <div style={{ padding: "12px" }}>
            <h3>Users</h3>

            {users.map(user => (
                <div 
                    key={user.userId}
                    onClick={() => navigate(`/users/${user.userId}`)}
                    style={{
                        padding: "8px",
                        color: user.isActive ? "inherit" : "#9ca3af",
                        cursor: "pointer",
                        borderBottom: "1px solid #eee"
                    }}
                >
                    <div>
                        <b>
                            {user.firstName} {user.lastName}
                        
                            {!user.isActive && (
                                <span 
                                    style={{
                                        marginLeft: "8px",
                                        fontSize: "11px",
                                        color: "#9ca3af",
                                        fontWeight: "normal"
                                    }}
                                >
                                    (inactive)
                                </span>
                            )}
                        </b>
                    </div>
                    
                    <div style={{ 
                        fontSize: "12px", 
                        color: user.isActive ? "#666" : "#9ca3af"
                    }}
                    >
                        @{user.login}
                    </div>
                </div>
            ))}
        </div>
    );
}