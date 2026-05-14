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

    const handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ) => {
        e.preventDefault();

        try {
            setLoading(true);

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
                    userId: "11111111-1111-1111-1111-111111111111",
                    title,
                    description
                };

                await createAchievement(dto);
            }

            onAchievementCreated();

            setTitle("");
            setDescription("");
        } catch (error) {
            console.error(error);
            alert("Operation failed");
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
                    required
                />
            </div>

            <button type="submit" disabled={loading}>
                {loading ? achievement ? "Saving..." : "Adding..." : achievement ? "Save changes" : "Add achievement"}
            </button>
        </form>
    );
}