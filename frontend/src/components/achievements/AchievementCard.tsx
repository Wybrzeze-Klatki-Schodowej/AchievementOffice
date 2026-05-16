import type { Achievement } from "../../types/achievement";
import { approveAchievement } from "../../api/achievementApi";
import { useState } from "react";

interface Props {
    achievement: Achievement;
    currentUserId: string;
}

export default function AchievementCard({ achievement, currentUserId }: Props) {
    const [vote, setVote] = useState<boolean | null>(null);
    const [loading, setLoading] = useState(false);

    const isOwner = achievement.userId === currentUserId;

    const handleVote = async (isApproved: boolean) => {
        if (loading || vote !== null) return;
        setLoading(true);
        try {
            await approveAchievement(achievement.achievementId, isApproved);
            setVote(isApproved);
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
            {!isOwner && (
                <div style={{ marginTop: "12px", display: "flex", gap: "8px" }}>
                    <button
                        onClick={() => handleVote(true)}
                        disabled={loading || vote !== null}
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
                        disabled={loading || vote !== null}
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