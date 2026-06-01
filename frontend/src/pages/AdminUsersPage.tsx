import { useCallback, useEffect, useState, useMemo } from "react";
import { getAllUsers, updateUserStatus, type AdminUserProfile } from "../api/AdminApi";
import "./AdminUsersPage.css";

type sortField = "login" | "firstName" | "email" | "role" | "isActive";

export default function AdminUsersPage() {
    const [users, setUsers] = useState<AdminUserProfile[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const [sortField, setSortField] = useState<sortField>("login");
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

    const fetchUsers = useCallback(async () => {
        setLoading(true);
        try {
            const data = await getAllUsers();
            setUsers(data);
            setError(null);
        } catch (err) {
            console.error(err);
            setError("Nie udało się pobrać listy użytkowników.");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const handleToggleStatus = async (userId: string, newStatus: boolean) => {
        const actionText = newStatus ? "aktywacji" : "dezaktywacji";
        try {
            await updateUserStatus(userId, newStatus);
            fetchUsers();
        } catch (err) {
            alert(`Błąd podczas ${actionText} użytkownika!`);
            console.error(err);
        }
    };

    const handleSort = (field: sortField) => {
        if (sortField === field) {
            setSortOrder(sortOrder === "asc" ? "desc" : "asc");
        } else {
            setSortField(field);
            setSortOrder("asc");
        }
    };

    const sortedUsers = useMemo(() => {
        return [...users].sort((a, b) => {
            let aValue = a[sortField];
            let bValue = b[sortField];

            if (typeof aValue === "string" && typeof bValue === "string") {
                return sortOrder === "asc"
                    ? aValue.localeCompare(bValue)
                    : bValue.localeCompare(aValue);
            } else if (typeof aValue === "boolean" && typeof bValue === "boolean") {
                return sortOrder === "asc"
                    ? (aValue === bValue ? 0 : aValue ? 1 : -1)
                    : (aValue === bValue ? 0 : aValue ? 1 : -1);
            }
            return 0;
        });
    }, [users, sortField, sortOrder]);

    if (loading) return <div className="admin-container">Ładowanie danych...</div>;
    if (error) return <div className="admin-container error-message">{error}</div>;

    return (
        <div className="admin-container">
            <div className="admin-header">
                <h1>Zarządzanie Użytkownikami</h1>
                <p>Przeglądaj konta i zatwierdzaj nowe rejestracje.</p>
            </div>

            <table className="admin-table">
                <thead>
                    <tr>
                        <th onClick={() => handleSort("login")} className="sortable-header">
                            Login
                        </th>
                        <th onClick={() => handleSort("firstName")} className="sortable-header">
                            Imię i Nazwisko
                        </th>
                        <th onClick={() => handleSort("email")} className="sortable-header">
                            Email
                        </th>
                        <th onClick={() => handleSort("role")} className="sortable-header">
                            Rola
                        </th>
                        <th onClick={() => handleSort("isActive")} className="sortable-header">
                            Status
                        </th>
                        <th>Akcje</th>
                    </tr>
                </thead>
                <tbody>
                    {sortedUsers.map((user) => (
                        <tr key={user.userId}>
                            <td className="font-bold">
                                <a href={`/users/${user.userId}`} className="user-login-link">
                                    {user.login}
                                </a>
                            </td>
                            <td>{`${user.firstName} ${user.lastName}`}</td>
                            <td>{user.email}</td>
                            <td><span className="role-badge">{user.role}</span></td>
                            <td>
                                <span className={`status-badge ${user.isActive ? "active" : "pending"}`}>
                                    {user.isActive ? "Aktywny" : "Oczekuje"}
                                </span>
                            </td>
                            <td>
                                {user.role !== "Admin" ? (
                                    user.isActive ? (
                                        <button
                                            className="deactivate-button"
                                            onClick={() => handleToggleStatus(user.userId, false)}
                                        >
                                            Dezaktywuj
                                        </button>
                                    ) : (
                                        <button
                                            className="approve-button"
                                            onClick={() => handleToggleStatus(user.userId, true)}
                                        >
                                            Zatwierdź
                                        </button>
                                    )
                                ) : (
                                    <span className="text-muted-action">—</span>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}