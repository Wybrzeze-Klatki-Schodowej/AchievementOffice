// src/pages/GroupsPage.tsx
import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getGroups, createGroup, type Group } from "../api/GroupApi";
import { getCurrentUser } from "../api/LoginApi";
import "./GroupsPage.css"; // We'll create this CSS next

export default function GroupsPage() {
    const [groups, setGroups] = useState<Group[]>([]);
    const [isAdmin, setIsAdmin] = useState(false);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [newGroup, setNewGroup] = useState({ name: "", description: "", maxUserCount: 100 });

    const navigate = useNavigate();

    useEffect(() => {
        fetchData();
    }, []);

    async function fetchData() {
        try {
            const user = await getCurrentUser();
            setIsAdmin(user.role === "Admin");
            const groupsData = await getGroups();
            setGroups(groupsData);
        } catch (err) {
            console.error(err);
        }
    }

    async function handleCreateGroup(e: React.FormEvent) {
        e.preventDefault();
        try {
            await createGroup(newGroup);
            setShowCreateModal(false);
            setNewGroup({ name: "", description: "", maxUserCount: 100 });
            fetchData(); // Refresh list
        } catch (err: any) {
            alert(err.message);
        }
    }

    return (
        <div className="groups-container">

            <div className="groups-header">
                <h1>Community Groups</h1>
                {isAdmin && (
                    <button className="create-group-btn" onClick={() => setShowCreateModal(true)}>
                        + Create Group
                    </button>
                )}
            </div>

            <div className="groups-grid">
                {groups.map(group => (
                    <Link to={`/groups/${group.groupId}`} key={group.groupId} className="group-card">
                        <h3>{group.name}</h3>
                        <p>{group.description}</p>
                        <span className="group-meta">Max Members: {group.maxUserCount}</span>
                    </Link>
                ))}
            </div>

            {showCreateModal && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h2>Create New Group</h2>
                        <form onSubmit={handleCreateGroup}>
                            <div className="form-group">
                                <label>Group Name</label>
                                <input required value={newGroup.name} onChange={e => setNewGroup({...newGroup, name: e.target.value})} />
                            </div>
                            <div className="form-group">
                                <label>Description</label>
                                <textarea required value={newGroup.description} onChange={e => setNewGroup({...newGroup, description: e.target.value})} />
                            </div>
                            <div className="form-group">
                                <label>Max Users</label>
                                <input type="number" required value={newGroup.maxUserCount} onChange={e => setNewGroup({...newGroup, maxUserCount: parseInt(e.target.value)})} />
                            </div>
                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowCreateModal(false)}>Cancel</button>
                                <button type="submit" className="save-btn">Create</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}