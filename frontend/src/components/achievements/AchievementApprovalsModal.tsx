import { useEffect, useState } from "react";
import { getAchievementApprovalsDetails } from "../../api/achievementApi";
import type { AchievementApprove } from "../../types/achievement";
import "./AchievementApprovalsModal.css";

interface Props {
    achievementId: string;
    onClose: () => void;
}

export default function AchievementApprovalsModal({
    achievementId,
    onClose
}: Props) {

    const [data, setData] = useState<{
        approved: AchievementApprove[];
        denied: AchievementApprove[];
    } | null>(null);

    const [loading, setLoading] = useState(true);

    useEffect(() => {
        load();
    }, [achievementId]);

    async function load() {
        try {
            setLoading(true);
            const res = await getAchievementApprovalsDetails(achievementId);
            setData(res);
        } catch (e) {
            console.error(e);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div 
            className="approval-modal-overlay"
            onClick={onClose}
        >
            <div 
                className="approval-modal"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="approval-modal-header">
                    <h3>Approvals</h3>
                    <button 
                        className="approval-modal-close"
                        onClick={onClose}
                    >
                        X
                    </button>
                </div>

                {loading && (
                    <p className="approval-loading">
                        Loading...
                    </p>
                )}

                {!loading && data && (
                    <>
                        <div className="approval-section">
                            <h4 className="approval-title approved">
                                Approved ({data.approved.length})
                            </h4>

                            {data.approved.length === 0 ? (
                                <p className="approval-empty">
                                    No approvals yet.
                                </p>
                            ) : (
                                data.approved.map((u) => (
                                    <div
                                        key={u.achievementApproveId}
                                        className="approval-user"
                                    >
                                        <div className="approval-name">
                                            {u.userFirstName} {u.userLastName}
                                        </div>

                                        <div className="approval-login">
                                            @{u.userLogin}
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>

                        <div className="approval-section">
                            <h4 className="approval-title denied">
                                Denied ({data.denied.length})
                            </h4>

                            {data.denied.length === 0 ? (
                                <p className="approval-empty">
                                    No denials.
                                </p>
                            ) : (
                                data.denied.map((u) => (
                                    <div
                                        key={u.achievementApproveId}
                                        className="approval-user"
                                    >
                                        <div className="approval-name">
                                            {u.userFirstName} {u.userLastName}
                                        </div>

                                        <div className="approval-login">
                                            @{u.userLogin}
                                        </div>
                                    </div>
                                ))
                            )}
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}