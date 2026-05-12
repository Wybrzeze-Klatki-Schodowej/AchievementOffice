import { useState } from "react";
import type { FormEvent } from "react";
import {
    createAchievement,
    type CreateAchievementDto,
} from "../../api/achievementApi";

interface Props {
    onAchievementCreated: () => void;
}

export default function AchievementForm({
    onAchievementCreated,
}: Props) {
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        try {
            setLoading(true);

            const dto: CreateAchievementDto = {
                userId: "11111111-1111-1111-1111-111111111111",
                title,
                description
            };

            await createAchievement(dto);

            setTitle("");
            setDescription("");

            onAchievementCreated();
        } catch (error) {
            console.error(error);
            alert("Failed to create achievement");
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Add achievement</h2>

            <div>
                <input 
                    type="text"
                    placeholder="Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
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
                {loading ? "Adding..." : "Add achievement"}
            </button>
        </form>
    );
}