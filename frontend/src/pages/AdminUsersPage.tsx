import { useCallback, useEffect, useState } from "react";
import { getAllUsers, updateUserStatus, type AdminUserProfile } from "../api/AdminApi";
import "./AdminUsersPage.css";

export default function AdminUsersPage() {
    const [users, setUsers] = useState<AdminUserProfile[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    //const [filterActive, setFilterActive] = useState<string>("all");

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

    const handleApprove = async (userId: string) => {
        try {
            await updateUserStatus(userId, true);
            fetchUsers();
        } catch (err) {
            alert("Błąd podczas aktywacji użytkownika!");
            console.error(err);
        }
    };

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
                        <th>Login</th>
                        <th>Imię i Nazwisko</th>
                        <th>Email</th>
                        <th>Rola</th>
                        <th>Status</th>
                        <th>Akcje</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map((user) => (
                        <tr key={user.userId}>
                            <td className="font-bold">{user.login}</td>
                            <td>{`${user.firstName} ${user.lastName}`}</td>
                            <td>{user.email}</td>
                            <td><span className="role-badge">{user.role}</span></td>
                            <td>
                                <span className={`status-badge ${user.isActive ? "active" : "pending"}`}>
                                    {user.isActive ? "Aktywny" : "Oczekuje"}
                                </span>
                            </td>
                            <td>
                                {!user.isActive && (
                                    <button
                                        className="approve-button"
                                        onClick={() => handleApprove(user.userId)}
                                    >
                                        Zatwierdź
                                    </button>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}