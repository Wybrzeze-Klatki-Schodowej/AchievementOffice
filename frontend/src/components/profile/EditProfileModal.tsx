import { useState } from "react";
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
        avatarUrl: user.avatarUrl ?? ""
    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

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
                avatarUrl: form.avatarUrl
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