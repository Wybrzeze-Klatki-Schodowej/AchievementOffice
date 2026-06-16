import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { getGroupById, getGroupMembers, removeGroupMember, type Group, type GroupMember } from "../api/GroupApi";
import { getCurrentUser } from "../api/LoginApi";
import AddGroupMemberModal from "../components/groups/AddGroupMemberModal";
import "./GroupsPage.css";

export default function GroupPage() {
    const { groupId } = useParams<{ groupId: string }>();
    const [group, setGroup] = useState<Group | null>(null);
    const [members, setMembers] = useState<GroupMember[]>([]);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [isGlobalAdmin, setIsGlobalAdmin] = useState(false);
    const [showAddMember, setShowAddMember] = useState(false);

    const navigate = useNavigate();

    useEffect(() => {
        if (groupId) fetchGroupData();
    }, [groupId]);

    async function fetchGroupData() {
        try {
            const me = await getCurrentUser();
            setCurrentUserId(me.userId);
            setIsGlobalAdmin(me.role === "Admin");

            const groupData = await getGroupById(groupId!);
            setGroup(groupData);

            const membersData = await getGroupMembers(groupId!);
            setMembers(membersData);
        } catch (err) {
            console.error(err);
        }
    }

    const myMembership = members.find(m => m.userId === currentUserId);
    const isGroupAdmin = isGlobalAdmin || myMembership?.isAdmin;

    async function handleRemoveMember(userId: string) {
        if (!window.confirm("Remove this user from the group?")) return; 
        try {
            await removeGroupMember(groupId!, userId);
            fetchGroupData();
        } catch (err: any) {
            alert(err.message);
        }
    }

    if (!group) return <div>Loading group...</div>;

    return (
        <div className="groups-container">
            <div className="back-navigation">
                <button className="back-link-btn" onClick={() => navigate('/groups')}>
                    &larr; Back to groups
                </button>
            </div>

            <div className="group-detail-header">
                <h1>{group.name}</h1>
                <p>{group.description}</p>
                <div className="group-meta">
                    Members: {members.length} / {group.maxUserCount}
                </div>
            </div>

            <div className="members-section">
                <div className="members-header">
                    <h2>Group Members</h2>
                    {isGroupAdmin && (
                        <button className="create-group-btn" onClick={() => setShowAddMember(true)}>
                            + Add Member
                        </button>
                    )}
                </div>

                <table className="admin-table">
                    <thead>
                        <tr>
                            <th>User</th>
                            <th>Role</th>
                            {isGroupAdmin && <th>Actions</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {members.map(member => (
                            <tr key={member.userId}>
                                <td>
                                    <Link to={`/users/${member.userId}`} className="user-login-link">
                                        {member.firstName} {member.lastName} (@{member.login})
                                    </Link>
                                </td>
                                <td>
                                    <span className={`role-badge ${member.isAdmin ? 'admin' : ''}`}>
                                        {member.roleName}
                                    </span>
                                </td>
                                {isGroupAdmin && (
                                    <td>
                                        {(
                                            <button className="deactivate-button" onClick={() => handleRemoveMember(member.userId)}>
                                                Remove
                                            </button>
                                        )}
                                    </td>
                                )}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {showAddMember && (
                <AddGroupMemberModal 
                    groupId={groupId!}
                    existingMembers={members}
                    onClose={() => setShowAddMember(false)}
                    onAdded={() => {
                        setShowAddMember(false);
                        fetchGroupData();
                    }}
                />
            )}
        </div>
    );
}