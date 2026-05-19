import { useEffect, useState } from "react";
import { getAllUsers } from "../../api/UserApi";
import { useNavigate } from "react-router-dom";

interface UserListItem {
    userId: string;
    login: string;
    firstName: string;
    lastName: string;
}

export default function UserList() {
    const [users, setUsers] = useState<UserListItem[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        getAllUsers()
            .then(setUsers)
            .catch(err => {
                console.error("Error fetching users:", err);
                alert("Failed to load users. Please try again later.");
            });
    }, []);

    return (
        <div style={{ padding: "12px" }}>
            <h3>Users</h3>

            {users.map(user => (
                <div 
                    key={user.userId}
                    onClick={() => navigate(`/users/${user.userId}`)}
                    style={{
                        padding: "8px",
                        cursor: "pointer",
                        borderBottom: "1px solid #eee"
                    }}
                >
                    <b>{user.firstName} {user.lastName}</b>
                    <div style={{ fontSize: "12px", color: "#666" }}>
                        @{user.login}
                    </div>
                </div>
            ))}
        </div>
    );
}