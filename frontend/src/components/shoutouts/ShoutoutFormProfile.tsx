import { useState } from "react";
import {
    createShoutout,
    updateShoutout,
    type CreateShoutoutDto,
} from "../../api/ShoutoutsApi";
import type { Shoutout } from "../../types/shoutout";

interface Props {
    onShoutoutCreated: () => void;
    receiverId: string;
    shoutout?: Shoutout;
}

export default function ShoutoutFormProfile({
    onShoutoutCreated,
    receiverId,
    shoutout,
}: Props) {
    const [title, setTitle] = useState(shoutout?.title ?? "");
    const [description, setDescription] = useState(shoutout?.description ?? "");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            setLoading(true);

            if (shoutout) {
                await updateShoutout(shoutout.shoutoutId, { title, description });
            } else {
                const dto: CreateShoutoutDto = { receiverId, title, description };
                await createShoutout(dto);
            }

            onShoutoutCreated();
            setTitle("");
            setDescription("");
        } catch (error) {
            console.error("SHOUTOUT ERROR:", error);
            alert(error instanceof Error ? error.message : "Operation failed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>{shoutout ? "Edit shoutout" : "Add shoutout"}</h2>

            <div>
                <input
                    type="text"
                    placeholder="Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    required
                />
            </div>

            <div>
                <textarea
                    placeholder="Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                />
            </div>

            <button type="submit" disabled={loading}>
                {loading
                    ? shoutout ? "Saving..." : "Adding..."
                    : shoutout ? "Save changes" : "Add shoutout"}
            </button>
        </form>
    );
}