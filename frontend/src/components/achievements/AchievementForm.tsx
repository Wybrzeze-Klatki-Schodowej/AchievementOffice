import { useState } from "react";
import {
    createAchievement,
    updateAchievement,
    type CreateAchievementDto,
} from "../../api/achievementApi";
import type { Achievement } from "../../types/achievement";

interface Props {
    onAchievementCreated: () => void;
    achievement?: Achievement;
}

export default function AchievementForm({
    onAchievementCreated,
    achievement,
}: Props) {
    const [title, setTitle] = useState(
        achievement?.title ?? ""
    );
    const [description, setDescription] = useState(
        achievement?.description ?? ""
    );
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ) => {
        e.preventDefault();

        try {
            setLoading(true);
            setError(null);

            if (achievement) {
                await updateAchievement(
                    achievement.achievementId, 
                    {
                        title,
                        description
                    }
                );
            } else {
                const dto: CreateAchievementDto = {
                    title,
                    description
                };

                await createAchievement(dto);
            }

            onAchievementCreated();

            setTitle("");
            setDescription("");
        } catch (error: any) {
            console.error(error);
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>{achievement ? "Edit achievement" : "Add achievement"}</h2>

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
                />
            </div>

            {error && (
                <p style={{ color: "red", marginTop: "8px" }}>
                    {error}
                </p>
            )}

            <button type="submit" disabled={loading}>
                {loading ? achievement ? "Saving..." : "Adding..." : achievement ? "Save changes" : "Add achievement"}
            </button>
        </form>
    );
}