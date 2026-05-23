import { useState } from "react";
import { changePassword } from "../../api/UserApi";

interface Props {
    onClose: () => void;
}

export default function ChangePasswordModal({ onClose }: Props) {

    const [form, setForm] = useState({
        currentPassword: "",
        newPassword: "",
        confirmNewPassword: ""
    });

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setSuccess(false);

        try {
            await changePassword(form);
            setSuccess(true);

            setTimeout(() => {
                onClose();
            }, 1000);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="modal-overlay">
            <div className="modal">

                <h2>Change password</h2>

                <form onSubmit={handleSubmit} className="modal-form">

                    <div className="form-group">
                        <label>Current password</label>
                        <input 
                            type="password"
                            name="currentPassword"
                            value={form.currentPassword}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>New password</label>
                        <input 
                            type="password"
                            name="newPassword"
                            value={form.newPassword}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>Confirm new password</label>
                        <input 
                            type="password"
                            name="confirmNewPassword"
                            value={form.confirmNewPassword}
                            onChange={handleChange}
                        />
                    </div>

                    {error && <p className="error">{error}</p>}
                    {success && <p className="success">Password changed successfully</p>}

                    <div className="modal-actions">
                        <button type="button" onClick={onClose}>
                            Cancel
                        </button>

                        <button type="submit" disabled={loading}>
                            {loading ? "Saving..." : "Change password"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}