import { useEffect, useState } from "react";
import { getAchievementApprovalsDetails } from "../../api/achievementApi";
import type { AchievementApprove } from "../../types/achievement";

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
        <div style={{
            position: "fixed",
            inset: 0,
            background: "rgba(0,0,0,0.5)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center"
        }}>
            <div style={{
                background: "white",
                padding: 20,
                borderRadius: 8,
                width: 500,
                maxHeight: "80vh",
                overflowY: "auto"
            }}>
                <div style={{ display: "flex", justifyContent: "space-between" }}>
                    <h3>Approvals</h3>
                    <button onClick={onClose}>X</button>
                </div>

                {loading && <p>Loading...</p>}

                {!loading && data && (
                    <>
                        <h4 style={{ color: "green" }}>Approved</h4>
                        {data.approved.map(u => (
                            <div key={u.achievementApproveId}>
                                {u.userLogin} ({u.userFirstName} {u.userLastName})
                            </div>
                        ))}

                        <h4 style={{ color: "red" }}>Denied</h4>
                        {data.denied.map(u => (
                            <div key={u.achievementApproveId}>
                                {u.userLogin} ({u.userFirstName} {u.userLastName})
                            </div>
                        ))}
                    </>
                )}
            </div>
        </div>
    );
}