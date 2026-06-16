import { useState, useEffect } from "react";
import { getAllUsers, type UserListItem } from "../../api/UserApi";
import { getGroupRoles, addGroupMember, type GroupRole, type GroupMember } from "../../api/GroupApi";
import "../../pages/GroupsPage.css";

interface AddGroupMemberModalProps {
    groupId: string;
    existingMembers: GroupMember[];
    onClose: () => void;
    onAdded: () => void;
}

export default function AddGroupMemberModal({ groupId, existingMembers, onClose, onAdded }: AddGroupMemberModalProps) {
    const [allUsers, setAllUsers] = useState<UserListItem[]>([]);
    const [groupRoles, setGroupRoles] = useState<GroupRole[]>([]);
    const [selectedUser, setSelectedUser] = useState("");
    const [selectedRole, setSelectedRole] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        async function fetchFormData() {
            try {
                const users = await getAllUsers();
                const roles = await getGroupRoles(groupId);
                
                const availableUsers = users.filter(u => !existingMembers.some(m => m.userId === u.userId));
                
                setAllUsers(availableUsers);
                setGroupRoles(roles);
            } catch (err) {
                console.error("Failed to load users/roles", err);
                setError("Failed to load form data.");
            }
        }
        fetchFormData();
    }, [groupId, existingMembers]);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (!selectedUser || !selectedRole) {
            setError("Please select both a user and a role.");
            return;
        }

        setIsLoading(true);
        setError(null);

        try {
            await addGroupMember(groupId, selectedUser, selectedRole);
            onAdded();
            onClose();
        } catch (err: any) {
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h2>Add Member to Group</h2>
                
                {error && <div style={{ color: "red", marginBottom: "10px" }}>{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Select User</label>
                        <select required value={selectedUser} onChange={e => setSelectedUser(e.target.value)}>
                            <option value="">-- Choose User --</option>
                            {allUsers.map(u => (
                                <option key={u.userId} value={u.userId}>
                                    {u.firstName} {u.lastName} (@{u.login})
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Select Role</label>
                        <select required value={selectedRole} onChange={e => setSelectedRole(e.target.value)}>
                            <option value="">-- Choose Role --</option>
                            {groupRoles.map(r => (
                                <option key={r.groupUserRoleId} value={r.groupUserRoleId}>
                                    {r.name} {r.isAdmin ? '(Admin)' : ''}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="modal-actions">
                        <button type="button" onClick={onClose} disabled={isLoading}>Cancel</button>
                        <button type="submit" className="save-btn" disabled={isLoading}>
                            {isLoading ? "Adding..." : "Add User"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}