import { useEffect, useState } from "react";
import { deleteShoutout, getShoutouts } from "../../api/ShoutoutsApi";
//import { deleteShoutout, getReceivedShoutouts } from "../../api/ShoutoutApi";
import type { Shoutout } from "../../types/shoutout";
import ShoutoutModal from "./ShoutoutModal";
import { getCurrentUser } from "../../api/LoginApi";
import ShoutoutCard from "./ShoutoutCard";

interface Props {
    userId: string;
    refreshTrigger: number;
}

export default function ShoutoutList({ refreshTrigger, userId }: Props) {
    const [shoutouts, setShoutouts] = useState<Shoutout[]>([]);
    const [loading, setLoading] = useState(true);
    const [currentUserId, setCurrentUserId] = useState<string | null>(null);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedShoutout, setSelectedShoutout] = useState<Shoutout | undefined>();

    const loadShoutouts = async () => {
        try {
            //setLoading(true);
            const data = await getShoutouts();
            const sorted = [...data]
                .filter(s => s.senderId === userId || s.receiverId === userId)
                .sort((a, b) =>
                    new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
                );
            setShoutouts(sorted);
            // setLoading(true);
            // const data = await getReceivedShoutouts(userId);            
            // setShoutouts(data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadShoutouts();
        getCurrentUser()
            .then((user) => setCurrentUserId(user.userId))
            .catch((error) => console.error("Failed to fetch current user:", error));
    }, [refreshTrigger, userId]);

    const handleDelete = async (id: string) => {
        try {
            await deleteShoutout(id);
            await loadShoutouts();
        } catch (error) {
            console.error(error);
            alert("Failed to delete shoutout");
        }
    };

    const handleEdit = (shoutout: Shoutout) => {
        setSelectedShoutout(shoutout);
        setIsEditModalOpen(true);
    };

    if (loading) {
        return <p>Loading shoutouts...</p>;
    }

    return (
        <div>
            <h2>Shoutouts</h2>
            <ShoutoutModal
                open={isEditModalOpen}
                shoutout={selectedShoutout}
                onClose={() => {
                    setIsEditModalOpen(false);
                    setSelectedShoutout(undefined);
                }}
                onShoutoutCreated={loadShoutouts}
            />
            {shoutouts.length === 0 && <p>No shoutouts yet.</p>}
            {shoutouts.map((shoutout) => (
                <ShoutoutCard
                    key={shoutout.shoutoutId}
                    shoutout={shoutout}
                    currentUserId={currentUserId}
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                    onReaction={loadShoutouts}
                />
            ))}
        </div>
    );
}
