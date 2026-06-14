import AchievementList from "../components/achievements/AchievementList";
import { useOutletContext, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import type { UserProfile } from "../types/user";
import { getUserProfile } from "../api/UserApi";
import EditProfileModal from "../components/profile/EditProfileModal";
import ChangePasswordModal from "../components/profile/ChangePasswordModal";
import ManageUserModal from "../components/profile/ManageUserModal";
import CommentSection from "../components/comments/CommentSection";
import "./ProfilePage.css";
import { getCurrentUser } from "../api/LoginApi";

interface OutletContext {
    refreshTrigger: number;
    refreshUsers: () => void;
    refreshNotifications: () => void;
}

export default function ProfilePage() {
    const { 
        refreshTrigger,
        refreshUsers,
        refreshNotifications
    } = useOutletContext<OutletContext>();
    const { userId } = useParams();

    const [user, setUser] = useState<UserProfile>();
    const [isEditOpen, setIsEditOpen] = useState(false);
    const [isPasswordOpen, setIsPasswordOpen] = useState(false);
    const [isManageUserOpen, setIsManageUserOpen] = useState(false);

    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [currentUserRole, setCurrentUserRole] = useState<string | null>(null);

    function handleUpdated(updated: UserProfile) {
        setUser(updated);
    }

    async function refreshProfile() {
        if (!userId) {
            return;
        }

        try {
            const updatedUser = await getUserProfile(userId);
            setUser(updatedUser);
            refreshUsers();
        } catch (error) {
            console.error(error);
        }
    }

    useEffect(() => {

        if (!userId) {
            return;
        }

        getUserProfile(userId)
            .then(setUser)
            .catch(console.error);
    }, [userId]);

    useEffect(() => {
        getCurrentUser()
            .then((user) => {
                setCurrentUserId(user.userId);
                setCurrentUserRole(user.role);
            })
            .catch((error) =>
                console.error("Failed to fetch current user:", error)
            );
    }, []);

    if (!user) {
        return <p>Loading...</p>;
    }

    const isOwnProfile = currentUserId === user.userId;
    const isAdmin = currentUserRole === "Admin";
    const canManageUser = isAdmin && !isOwnProfile;

    return (
        <div className={`
            profile-container 
            ${isOwnProfile ? "profile-own" : ""}
            ${!user.isActive ? "profile-inactive" : ""}
            `}
        >
            <div className="profile-card">
                <div className="profile-header">
                    <div>
                        <h1 className="profile-name">
                            {user.firstName}
                            {" "}
                            {user.lastName}
                        </h1>
                    <div className="profile-username">
                        @{user.login}
                    </div>

                    {isOwnProfile && (
                        <div className="profile-badge">
                            Your profile
                        </div>
                    )}
                    {!user.isActive && (
                        <div className="profile-badge-inactive">
                            Inactive user
                        </div>
                    )}
                    {isOwnProfile && (
                        <button onClick={() => setIsEditOpen(true)}>
                            Edit profile
                        </button>
                    )}
                    {isOwnProfile && (
                        <button onClick={() => setIsPasswordOpen(true)}>
                            Change password
                        </button>
                    )}
                    {canManageUser && (
                        <button 
                            onClick={() => setIsManageUserOpen(true)}
                        >
                            Manage user
                        </button>
                    )}
                </div>
            </div>

            <div className="profile-meta">
                <div><b>Email:</b> {user.email}</div>
                <div><b>Job Title:</b> {user.jobTitle}</div>
                <div><b>Bio:</b> {user.bio}</div>
                <div>
                    <b>User since:</b>{" "}
                    {new Date(user.createdAt).toLocaleDateString()}
                </div>
            </div>
        </div>

        <div className="profile-card">

            <AchievementList
                userId={userId!}
                refreshTrigger={refreshTrigger}
                onNotificationsRefresh={refreshNotifications}
            />

        </div>

        <CommentSection 
            profileUserId={userId!}
        />

        {(isAdmin || isOwnProfile) && isEditOpen && user && (
            <EditProfileModal
                user={user}
                onClose={() => setIsEditOpen(false)}
                onUpdated={handleUpdated}
            />
        )}

        {isOwnProfile && isPasswordOpen && (
            <ChangePasswordModal
                onClose={() => setIsPasswordOpen(false)}
            />
        )}

        {canManageUser && isManageUserOpen && (
            <ManageUserModal 
                userId={user.userId}
                isActive={user.isActive}
                onClose={() => setIsManageUserOpen(false)}
                onUpdated={refreshProfile}
            />
        )}
    </div>
    );
}