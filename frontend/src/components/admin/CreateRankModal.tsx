import React, { useState } from "react";
import { createRank } from "../../api/AdminApi";
import "./CreateRankModal.css";

interface CreateRankModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => Promise<void>;
}

export default function CreateRankModal({ isOpen, onClose, onSuccess }: CreateRankModalProps) {
    const [rankName, setRankName] = useState<string>("");
    const [rankMultiplier, setRankMultiplier] = useState<number>(1.0);
    const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

    if (!isOpen) return null;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!rankName.trim()) return;

        setIsSubmitting(true);
        try {
            await createRank({
                name: rankName,
                multiplier: rankMultiplier
            });

            setRankName("");
            setRankMultiplier(1.0);
            onClose();
            await onSuccess();
        } catch (err: any) {
            console.error(err);
            alert(err.message || "An error occurred while creating the rank");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="rank-modal-overlay" onClick={onClose}>
            <div className="rank-modal-content" onClick={(e) => e.stopPropagation()}>
                <h3>Create New Rank</h3>
                <form onSubmit={handleSubmit}>
                    <div className="rank-form-group">
                        <label htmlFor="modal-rank-name">Rank Name</label>
                        <input
                            id="modal-rank-name"
                            type="text"
                            value={rankName}
                            onChange={(e) => setRankName(e.target.value)}
                            placeholder="e.g. Diamond"
                            required
                        />
                    </div>
                    <div className="rank-form-group">
                        <label htmlFor="modal-rank-multiplier">Multiplier</label>
                        <input
                            id="modal-rank-multiplier"
                            type="number"
                            step="0.01"
                            min="1.0"
                            value={rankMultiplier}
                            onChange={(e) => setRankMultiplier(parseFloat(e.target.value) || 1.0)}
                            required
                        />
                    </div>
                    <div className="rank-modal-actions">
                        <button
                            type="button"
                            className="rank-cancel-btn"
                            onClick={onClose}
                            disabled={isSubmitting}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className="rank-submit-btn"
                            disabled={isSubmitting}
                        >
                            Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}