import type { Shoutout } from "../../types/shoutout";

interface Props {
    shoutout: Shoutout;
    currentUserId: string | null;
    onEdit: (shoutout: Shoutout) => void;
    onDelete: (id: string) => void;
}

export default function ShoutoutCard({ shoutout, currentUserId, onEdit, onDelete }: Props) {
    const senderName = `${shoutout.senderFirstname} ${shoutout.senderLastname}`.trim() || shoutout.senderLogin;
    const receiverName = `${shoutout.receiverFirstname} ${shoutout.receiverLastname}`.trim() || shoutout.receiverLogin;

    return (
        <div
            style={{
                border: "1px solid #ccc",
                padding: "16px",
                borderRadius: "8px",
                marginBottom: "12px",
            }}
        >
            <h3>{shoutout.title}</h3>
            <p>
                <strong>{senderName}</strong> → <strong>{receiverName}</strong>
            </p>
            <p>{shoutout.description}</p>
            
            <small>
                Created: {new Date(shoutout.createdAt).toLocaleString()}
                <br />
                Updated: {new Date(shoutout.updatedAt).toLocaleString()}
            </small>
            <div style={{ marginTop: "8px" }}>
                {currentUserId === shoutout.senderId && (
                    <div style={{ marginTop: "8px" }}>
                        <button onClick={() => onEdit(shoutout)}>Edit</button>
                        <button onClick={() => onDelete(shoutout.shoutoutId)} style={{ marginLeft: "8px" }}>
                            Delete
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

