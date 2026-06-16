import { useEffect, useState } from "react";
import type { UserReviewer } from "../../types/achievementVerificationRequest";
import {
    getAvailableReviewers,
    createVerificationRequest
} from "../../api/AchievementVerificationRequestApi";
import "./SelectReviewerModal.css";

interface Props {
    achievementId: string;
    onClose: () => void;
    onSuccess: () => void;
}

export default function SelectReviewerModal({
    achievementId,
    onClose,
    onSuccess
}: Props) {

    const [reviewers, setReviewers] = useState<UserReviewer[]>([]);
    const [loading, setLoading] = useState(true);
    const [sending, setSending] = useState(false);

    useEffect(() => {
        async function load() {
            try {
                const data = await getAvailableReviewers(achievementId);
                setReviewers(data);
            } catch (e) {
                console.error(e);
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [achievementId]);

    const handleSelect = async (userId: string) => {
        if (sending) return;

        setSending(true);
        try {
            await createVerificationRequest(achievementId, {
                targetUserId: userId
            });

            onSuccess();
            onClose();
        } catch (e) {
            console.error(e);
        } finally {
            setSending(false);
        }
    };

    if (loading) return <div className="modal-overlay">Loading reviewers...</div>;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                
                <button className="modal-close" onClick={onClose}>
                    X
                </button>

                <h3 className="modal-title">
                    Select reviewer
                </h3>

                <div className="modal-body">
                    {reviewers.length === 0 && (
                        <p className="empty-state">
                            No available reviewers
                        </p>
                    )}

                    {reviewers.map(r => (
                        <div key={r.userId} className="reviewer-item">
                            <div>
                                <b>{r.firstName} {r.lastName}</b>
                                <div className="reviewer-login">@{r.login}</div>
                            </div>

                            <button
                                disabled={sending}
                                onClick={() => handleSelect(r.userId)}
                            >
                                Select
                            </button>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}