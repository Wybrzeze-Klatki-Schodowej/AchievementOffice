import { useState } from "react";
import { addReaction, removeReaction } from "../../api/ShoutoutApi";
import type { Shoutout } from "../../types/shoutout";
import "./ShoutoutCard.css";

interface Props {
    shoutout: Shoutout;
    currentUserId: string | null;
    onEdit: (shoutout: Shoutout) => void;
    onDelete: (id: string) => void;
    onReaction?: () => void;
}

const REACTION_MAP = [
    { type: 0, emoji: "👍", label: "Like" },
    { type: 1, emoji: "❤️", label: "Love" },
    { type: 2, emoji: "😂", label: "Haha" },
    { type: 3, emoji: "😮", label: "Wow" },
    { type: 4, emoji: "😢", label: "Sad" },
    { type: 5, emoji: "😠", label: "Angry" },
];

export default function ShoutoutCard({ shoutout, currentUserId, onEdit, onDelete, onReaction }: Props) {
    const [isReacting, setIsReacting] = useState(false);

    const senderName = `${shoutout.senderFirstname} ${shoutout.senderLastname}`.trim() || shoutout.senderLogin;

    const handleReactionClick = async (reactionType: number) => {
        if (!currentUserId || isReacting) return;

        try {
            setIsReacting(true);
            if (shoutout.currentUserReaction && REACTION_MAP.find(r => r.emoji === shoutout.currentUserReaction)?.type === reactionType) {
                await removeReaction(shoutout.shoutoutId);
            } else {
                await addReaction(shoutout.shoutoutId, reactionType);
            }
            onReaction?.();
        } catch (error) {
            console.error("Failed to react:", error);
        } finally {
            setIsReacting(false);
        }
    };

    return (
        <div className="shoutout-card">
            <div className="shoutout-header">
                <h3>{shoutout.title}</h3>
                <div className="shoutout-meta">
                    <span className="meta-label">From</span>
                    <span className="sender-name">
                        {senderName}
                    </span>

                    {shoutout.senderFirstname && shoutout.senderLastname && (
                        <span className="sender-login">
                            @{shoutout.senderLogin}
                        </span>
                    )}
                </div>
            </div>

            <p className="shoutout-description">{shoutout.description}</p>

            <div className="shoutout-footer">
                <small>
                    Created: {new Date(shoutout.createdAt).toLocaleString()}
                    <br />
                    Updated: {new Date(shoutout.updatedAt).toLocaleString()}
                </small>

                <div className="reaction-bar">
                    {REACTION_MAP.map((r) => (
                        <button
                            key={r.type}
                            className={`reaction-btn ${shoutout.currentUserReaction === r.emoji ? "active" : ""}`}
                            onClick={() => handleReactionClick(r.type)}
                            disabled={!currentUserId || isReacting}
                            title={r.label}
                        >
                            <span className="emoji">{r.emoji}</span>
                            {shoutout.reactions && shoutout.reactions[r.emoji] > 0 && (
                                <span className="count">{shoutout.reactions[r.emoji]}</span>
                            )}
                        </button>
                    ))}
                </div>

                {currentUserId === shoutout.senderId && (
                    <div className="actions">
                        <button className="edit-btn" onClick={() => onEdit(shoutout)}>Edit</button>
                        <button className="delete-btn" onClick={() => onDelete(shoutout.shoutoutId)}>
                            Delete
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

