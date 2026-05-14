import AchievementForm from "./AchievementForm";
import "./AchievementModal.css";

interface Props {
    open: boolean;
    onClose: () => void;
    onAchievementCreated: () => void;
}

export default function AchievementModal({
    open,
    onClose,
    onAchievementCreated,
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
                    onAchievementCreated={() => {
                        onAchievementCreated();
                        onClose();
                    }}
                />
            </div>
        </div>
    );
}