import AchievementForm from "./AchievementForm";
import type { Achievement } from "../../types/achievement";
import "./AchievementModal.css";

interface Props {
    open: boolean;
    onClose: () => void;
    onAchievementCreated: () => void;

    achievement?: Achievement;
}

export default function AchievementModal({
    open,
    onClose,
    onAchievementCreated,
    achievement,
}: Props) {
    if (!open) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <button
                    className="close-button"
                    onClick={onClose}
                >
                    X
                </button>

                <AchievementForm 
                    achievement={achievement}
                    onAchievementCreated={() => {
                        onAchievementCreated();
                        onClose();
                    }}
                />
            </div>
        </div>
    );
}