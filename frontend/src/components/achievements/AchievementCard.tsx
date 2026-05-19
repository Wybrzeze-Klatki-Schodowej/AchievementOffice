import type { Achievement, AchievementApprovalSummary } from "../../types/achievement";
import { approveAchievement, getApprovalSummary } from "../../api/achievementApi";
import { useState, useEffect, useCallback } from "react";

interface Props {
    achievement: Achievement;
    currentUserId: string | null;
}

export default function AchievementCard({ achievement, currentUserId }: Props) {
    const [vote, setVote] = useState<boolean | null>(null);
    const [loading, setLoading] = useState(false);
    const [summary, setSummary] = useState<AchievementApprovalSummary>({ approved: 0, denied: 0 });

    const fetchSummary = useCallback(() => {
        getApprovalSummary(achievement.achievementId)
            .then(setSummary)
            .catch(console.error);
    }, [achievement.achievementId]);

    useEffect(() => {
        fetchSummary();
    }, [fetchSummary]);

    const isOwner = achievement.userId === currentUserId;

    const handleVote = async (isApproved: boolean) => {
        if (loading) return;
        setLoading(true);
        try {
            const isUnvoting = vote === isApproved;

            await approveAchievement(achievement.achievementId, isApproved);

            setVote(isUnvoting ? null : isApproved);
            fetchSummary();
        } catch (e) {
            console.error(e);
        } finally {
            setLoading(false);
        }
    };
    return (
        <div
            style={{
                border: "1px solid #ccc",
                padding: "16px",
                borderRadius: "8px",
                marginBottom: "12px",
            }}
        >
            <h3>{achievement.title}</h3>

            <p>{achievement.description}</p>

            <small>
                Created: {new Date(achievement.createdAt).toLocaleString()}
                <br />
                Updated: {new Date(achievement.updatedAt).toLocaleString()}
            </small>

            <div style={{ marginTop: "8px", fontSize: "14px" }}>
                 {summary.approved} approves &nbsp;  {summary.denied} denies
            </div>

            {!isOwner && (
                <div style={{ marginTop: "12px", display: "flex", gap: "8px" }}>
                    <button
                        onClick={() => handleVote(true)}
                        disabled={loading}
                        style={{
                            background: vote === true ? "green" : "#eee",
                            color: vote === true ? "white" : "black",
                            border: "1px solid #ccc",
                            padding: "6px 12px",
                            borderRadius: "4px",
                            cursor: vote !== null ? "default" : "pointer",
                        }}
                    >
                        Approve                    </button>
                    <button
                        onClick={() => handleVote(false)}
                        disabled={loading}
                        style={{
                            background: vote === false ? "red" : "#eee",
                            color: vote === false ? "white" : "black",
                            border: "1px solid #ccc",
                            padding: "6px 12px",
                            borderRadius: "4px",
                            cursor: vote !== null ? "default" : "pointer",
                        }}
                    >
                        Deny
                    </button>
                </div>
            )}
        </div>
    );
}