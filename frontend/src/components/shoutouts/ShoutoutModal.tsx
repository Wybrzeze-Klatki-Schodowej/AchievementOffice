import ShoutoutForm from "./ShoutoutForm.tsx";
import ShoutoutFormProfile from "./ShoutoutFormProfile.tsx";
import type { Shoutout } from "../../types/shoutout";
import "./ShoutoutModal.css";

interface Props {
    open: boolean;
    onClose: () => void;
    onShoutoutCreated: () => void;
    shoutout?: Shoutout;
    receiverId?: string;
}

export default function ShoutoutModal({
    open,
    onClose,
    onShoutoutCreated,
    shoutout,
    receiverId,
}: Props) {
    if (!open) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <button className="close-button" onClick={onClose}>
                    X
                </button>

                {receiverId ? (
                    <ShoutoutFormProfile
                        receiverId={receiverId}
                        shoutout={shoutout}
                        onShoutoutCreated={() => {
                            onShoutoutCreated();
                            onClose();
                        }}
                    />
                ) : (
                    <ShoutoutForm
                        shoutout={shoutout}
                        onShoutoutCreated={() => {
                            onShoutoutCreated();
                            onClose();
                        }}
                    />
                )}
            </div>
        </div>
    );
}