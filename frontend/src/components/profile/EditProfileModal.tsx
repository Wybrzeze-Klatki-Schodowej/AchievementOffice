import { useState, useEffect } from "react";
import type { UserProfile } from "../../types/user";
import { updateUserProfile } from "../../api/UserApi";
import "./EditProfileModal.css";

interface Props {
    user: UserProfile;
    onClose: () => void;
    onUpdated: (user: UserProfile) => void;
}

export default function EditProfileModal({
    user,
    onClose,
    onUpdated
}: Props) {

    const [form, setForm] = useState({
        email: user.email,
        username: user.login,
        firstname: user.firstName,
        lastname: user.lastName,
        jobTitle: user.jobTitle,
        bio: user.bio ?? "",
        avatarUrl: user.avatarUrl ?? "",
        visibilityId: user.visibilityId,
        groupIds: user.groupIds ?? [],
    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [visibilityId, setVisibilityId] = useState<number>(
        user?.visibilityId ?? 1
    );
    const [groupIds, setGroupIds] = useState<string[]>(user?.groupIds ?? []);
    const [groups, setGroups] = useState<{ groupId: string; name: string }[]>([]);

    useEffect(() => {
        const fetchGroups = async () => {
            try {
                const res = await fetch(import.meta.env.VITE_API_URL + "/groups", {
                    credentials: "include",
                });

                if (!res.ok) throw new Error("Failed to fetch groups");

                const data = await res.json();
                setGroups(data);
            } catch (e) {
                console.error(e);
            }
        };

        fetchGroups();
    }, []);

    function handleChange(
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
    ) {
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            const updated = await updateUserProfile({
                email: form.email,
                username: form.username,
                firstname: form.firstname,
                lastname: form.lastname,
                jobTitle: form.jobTitle,
                bio: form.bio,
                avatarUrl: form.avatarUrl,
                visibilityId: visibilityId,
                groupIds: visibilityId === 3 ? groupIds : [],
                isProfileRestricted: false,
            });

            onUpdated(updated);
            onClose();
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="modal-overlay">
            <div className="modal">
                <h2>Edit profile</h2>

                <form onSubmit={handleSubmit} className="modal-form">

                    <div className="form-group">
                        <label htmlFor="email">Email</label>
                        <input
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            placeholder="Email"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="username">Username</label>
                        <input
                            name="username"
                            value={form.username}
                            onChange={handleChange}
                            placeholder="Username"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="firstname">First name</label>
                        <input
                            name="firstname"
                            value={form.firstname}
                            onChange={handleChange}
                            placeholder="First name"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="lastname">Last name</label>
                        <input
                            name="lastname"
                            value={form.lastname}
                            onChange={handleChange}
                            placeholder="Last name"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="jobTitle">Job title</label>
                        <input
                            name="jobTitle"
                            value={form.jobTitle}
                            onChange={handleChange}
                            placeholder="Job title"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="bio">Bio</label>
                        <textarea
                            name="bio"
                            value={form.bio}
                            onChange={handleChange}
                            placeholder="Bio"
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="visibility">Visibility</label>
                        <select
                            id="visibility"
                            value={visibilityId}
                            onChange={(e) => setVisibilityId(Number(e.target.value))}
                        >
                            <option value={1}>Public</option>
                            <option value={2}>Private</option>
                            <option value={3}>Groups</option>
                        </select>
                    </div>

                    {visibilityId === 3 && (
                        <div className="form-group">
                            <label>Allowed groups</label>

                            {groups.map((g) => (
                                <label key={g.groupId} style={{ display: "block" }}>
                                    <input
                                        type="checkbox"
                                        checked={groupIds.includes(g.groupId)}
                                        onChange={(e) => {
                                            if (e.target.checked) {
                                                setGroupIds([...groupIds, g.groupId]);
                                            } else {
                                                setGroupIds(groupIds.filter(id => id !== g.groupId));
                                            }
                                        }}
                                    />
                                    {g.name}
                                </label>
                            ))}
                        </div>
                    )}

                    {error && (
                        <pre
                            className="error-message"
                            style={{
                                color: 'red',
                                margin: '1rem 0',
                                whiteSpace: 'pre-wrap',
                                fontFamily: 'inherit'
                            }}
                        >
                            {error}
                        </pre>
                    )}

                    <div className="modal-actions">
                        <button
                            type="button"
                            onClick={onClose}
                        >
                            Cancel
                        </button>

                        <button
                            type="submit"
                            disabled={loading}
                        >
                            {loading ? "Saving..." : "Save"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}