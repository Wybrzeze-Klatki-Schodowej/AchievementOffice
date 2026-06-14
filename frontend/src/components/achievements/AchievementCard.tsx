import type { Achievement, AchievementApprovalSummary } from "../../types/achievement";
import { approveAchievement, getApprovalSummary } from "../../api/achievementApi";
import { useState, useEffect, useCallback } from "react";
import SelectReviewerModal from "./SelectReviewerModal";
import AchievementApprovalsModal from "./AchievementApprovalsModal";

interface Props {
    achievement: Achievement;
    currentUserId: string | null;
    currentUserRole?: string | null;
    onEdit?: (achievement: Achievement) => void;
    onDelete?: (id: string) => void;
    onVoteCompleted?: () => void;
}

export default function AchievementCard({ 
    achievement, 
    currentUserId, 
    currentUserRole, 
    onEdit, 
    onDelete,
    onVoteCompleted
}: Props) {

    const [loading, setLoading] = useState(false);
    const [showReviewerModal, setShowReviewerModal] = useState(false);
    const [showApprovals, setShowApprovals] = useState(false);

    const [summary, setSummary] = 
        useState<AchievementApprovalSummary>({ 
            approved: 0, 
            denied: 0,
            currentUserVote: null
        });

    const fetchSummary = useCallback(async () => {
        const data = 
            await getApprovalSummary(
                achievement.achievementId
            );

        setSummary(data);
    }, [achievement.achievementId]);

    useEffect(() => {
        fetchSummary();
    }, [fetchSummary]);

    const isOwner = achievement.userId === currentUserId;
    const isAdmin = currentUserRole === "Admin";

    const canEdit = isOwner || isAdmin;

    const handleVote = async (isApproved: boolean) => {
        if (loading) return;

        setLoading(true);
        try {
            await approveAchievement(achievement.achievementId, isApproved);

            await fetchSummary();

            onVoteCompleted?.();
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

            <div
                style={{
                    marginTop: "8px",
                    fontSize: "14px",
                    cursor: "pointer",
                    textDecoration: "underline"
                }}
                onClick={() => setShowApprovals(true)}
            >
                {summary.approved} approves · {summary.denied} denies (view details)
            </div>

            {!isOwner && (
                <div style={{ marginTop: "12px", display: "flex", gap: "8px" }}>
                    <button
                        onClick={() => handleVote(true)}
                        disabled={loading}
                        style={{
                            background: summary.currentUserVote === true ? "green" : "#eee",
                            color: summary.currentUserVote === true ? "white" : "black",
                            border: "1px solid #ccc",
                            padding: "6px 12px",
                            borderRadius: "4px",
                            cursor: "pointer",
                        }}
                    >
                        Approve                    </button>
                    <button
                        onClick={() => handleVote(false)}
                        disabled={loading}
                        style={{
                            background: summary.currentUserVote === false ? "red" : "#eee",
                            color: summary.currentUserVote === false ? "white" : "black",
                            border: "1px solid #ccc",
                            padding: "6px 12px",
                            borderRadius: "4px",
                            cursor: "pointer",
                        }}
                    >
                        Deny
                    </button>
                </div>
            )}

            {canEdit && (
                <div style={{ marginTop: "12px", display: "flex", gap: "8px" }}>
                    <button onClick={() => onEdit?.(achievement)}>
                        Edit
                    </button>
                    <button onClick={() => onDelete?.(achievement.achievementId)}>
                        Delete
                    </button>
                </div>
            )}

            {isOwner && (
                <div style={{ marginTop: 12 }}>
                    <button onClick={() => setShowReviewerModal(true)}>
                        Request verification
                    </button>
                </div>
            )}

            {showReviewerModal && (
                <SelectReviewerModal 
                    achievementId={achievement.achievementId}
                    onClose={() => setShowReviewerModal(false)}
                    onSuccess={fetchSummary}
                />
            )}

            {showApprovals && (
                <AchievementApprovalsModal 
                    achievementId={achievement.achievementId}
                    onClose={() => setShowApprovals(false)}
                />
            )}
        </div>
    );
}