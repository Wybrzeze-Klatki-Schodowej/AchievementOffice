import { updateUserStatus } from "../../api/AdminApi";

interface Props {
    userId: string;
    isActive: boolean;
    onClose: () => void;
    onUpdated: () => void;
}

export default function ManageUserModal({
    userId,
    isActive,
    onClose,
    onUpdated
}: Props) {

    async function handleStatusChange() {
        await updateUserStatus(
            userId, 
            !isActive
        );
        onUpdated();
        onClose();
    }

    return (
        <div className="modal-overlay">
            <div className="modal">

                <h2>
                    User management
                </h2>

                <p>
                    Are you sure you want to {" "}
                    {isActive 
                        ? "deactivate" 
                        : "activate"}{" "} 
                    this account?
                </p>

                <div className="modal-actions">

                    <button
                        onClick={onClose}
                    >
                        Cancel
                    </button>

                    <button
                        onClick={handleStatusChange}
                    >
                        {isActive ? "Deactivate" : "Activate"}
                    </button>
                </div>
            </div>
        </div>
    );
}