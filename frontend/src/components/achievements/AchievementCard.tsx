import type { Achievement, AchievementApprovalSummary } from "../../types/achievement";
import { approveAchievement, getApprovalSummary } from "../../api/achievementApi";
import { useState, useEffect, useCallback } from "react";
import SelectReviewerModal from "./SelectReviewerModal";
import AchievementApprovalsModal from "./AchievementApprovalsModal";
import "./AchievementCard.css";

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
        <div className="achievement-card">
            <h3>{achievement.title}</h3>

            <p>{achievement.description}</p>

            <small>
                Created: {new Date(achievement.createdAt).toLocaleString()}
                <br />
                Updated: {new Date(achievement.updatedAt).toLocaleString()}
            </small>

            <div
                className="achievement-approvals-link"
                onClick={() => setShowApprovals(true)}
            >
                {summary.approved} approves · {summary.denied} denies (view details)
            </div>

            {!isOwner && (
                <div className="achievement-vote-buttons">
                    <button
                        onClick={() => handleVote(true)}
                        disabled={loading}
                        className={`vote-button ${
                            summary.currentUserVote === true
                                ? "vote-button-active-approve"
                                : "vote-button-inactive"
                        }`}
                    >
                        Approve                    </button>
                    <button
                        onClick={() => handleVote(false)}
                        disabled={loading}
                        className={`vote-button ${
                            summary.currentUserVote === false
                                ? "vote-button-active-deny"
                                : "vote-button-inactive"
                        }`}
                    >
                        Deny
                    </button>
                </div>
            )}

            {canEdit && (
                <div className="achievement-action-buttons">
                    <button onClick={() => onEdit?.(achievement)}>
                        Edit
                    </button>
                    <button onClick={() => onDelete?.(achievement.achievementId)}>
                        Delete
                    </button>
                </div>
            )}

            {isOwner && (
                <div className="achievement-request-verification">
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