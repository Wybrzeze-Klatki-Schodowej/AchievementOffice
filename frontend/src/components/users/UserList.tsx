import { useEffect, useState } from "react";
import { getAllUsers } from "../../api/UserApi";
import { useNavigate } from "react-router-dom";
import UserSearch from "./UserSearch";

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
    const [searchTerm, setSearchTerm] = useState("");

    const navigate = useNavigate();

    useEffect(() => {
        getAllUsers()
            .then(setUsers)
            .catch(err => {
                console.error("Error fetching users:", err);
                alert("Failed to load users. Please try again later.");
            });
    }, [refreshTrigger]);

    const sortedUsers = [...users].sort((a, b) => {
        const lastNameCompare =
            a.lastName.localeCompare(
                b.lastName,
                undefined,
                {
                    sensitivity: "base"
                }
            );
        
        if (lastNameCompare !== 0) {
            return lastNameCompare;
        }

        return a.firstName.localeCompare(
            b.firstName,
            undefined,
            {
                sensitivity: "base"
            }
        );
    });

    const filteredUsers = sortedUsers.filter(user => {
        const query =
            searchTerm
                .trim()
                .toLowerCase();

        if (!query) {
            return true;
        }

        const fullName =
            `${user.firstName} ${user.lastName}`
                .toLowerCase();

        const reversedName =
            `${user.lastName} ${user.firstName}`
                .toLowerCase();

        const login =
            user.login.toLowerCase();

        return (
            fullName.includes(query) ||
            reversedName.includes(query) ||
            login.includes(query)
        );
    });

    return (
        <div style={{ padding: "12px" }}>
            <h3>Users</h3>

            <UserSearch 
                value={searchTerm}
                onChange={setSearchTerm}
            />

            {filteredUsers.map(user => (
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
                            {user.firstName} 
                            {" "}
                            {user.lastName}
                        
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

            {filteredUsers.length === 0 && (
                <div 
                    style={{
                        padding: "12px 0",
                        color: "#9ca3af",
                        textAlign: "center"
                    }}
                >
                    No users found
                </div>
            )}
        </div>
    );
}