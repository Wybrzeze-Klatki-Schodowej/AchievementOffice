import ShoutoutForm from "./ShoutoutForm.tsx";
import type { Shoutout } from "../../types/shoutout";
import "./ShoutoutModal.css";

interface Props {
    open: boolean;
    onClose: () => void;
    onShoutoutCreated: () => void;

    shoutout?: Shoutout;
}

export default function ShoutoutModal({
    open,
    onClose,
    onShoutoutCreated,
    shoutout,
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

                <ShoutoutForm 
                    shoutout={shoutout}
                    onShoutoutCreated={() => {
                        onShoutoutCreated();
                        onClose();
                    }}
                />
            </div>
        </div>
    );
}