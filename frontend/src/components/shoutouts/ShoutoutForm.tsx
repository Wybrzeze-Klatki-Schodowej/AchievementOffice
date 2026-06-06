import { useEffect, useState } from "react";
import {
    createShoutout,
    updateShoutout,
    type CreateShoutoutDto,
} from "../../api/ShoutoutsApi";
import type { Shoutout } from "../../types/shoutout";
import { getCurrentUser } from "../../api/LoginApi";

interface User {
    userId: string;
    login: string;
}

interface Props {
    onShoutoutCreated: () => void;
    shoutout?: Shoutout;
}

export default function ShoutoutForm({
    onShoutoutCreated,
    shoutout,
}: Props) {
    const [title, setTitle] = useState(shoutout?.title ?? "");
    const [description, setDescription] = useState(shoutout?.description ?? "");
    const [receiverId, setReceiverId] = useState("");
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const response = await fetch(
                    "http://localhost:8080/api/users",
                    {
                        credentials: "include",
                    }
                );

                console.log("GET users status:", response.status);

                if (!response.ok) {
                    throw new Error("Failed to fetch users");
                }

                const data: User[] = await response.json();

                const current = await getCurrentUser();

                const filteredUsers = data.filter(u => u.userId !== current.userId);
                setUsers(filteredUsers);

                if (filteredUsers.length > 0) {
                    setReceiverId(filteredUsers[0].userId);
                }
            } catch (error) {
                console.error("FETCH USERS ERROR:", error);
            }
        };

        fetchUsers();
    }, []);

    const handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ) => {
        e.preventDefault();

        try {
            setLoading(true);

            if (!receiverId) {
                throw new Error("No receiver selected");
            }

            if (shoutout) {
                await updateShoutout(shoutout.shoutoutId, {
                    title,
                    description,
                });
            } else {
                const dto: CreateShoutoutDto = {
                    receiverId,
                    title,
                    description,
                };

                console.log("Creating shoutout:", dto);

                await createShoutout(dto);
            }

            onShoutoutCreated();

            setTitle("");
            setDescription("");

            if (users.length > 0) {
                setReceiverId(users[0].userId);
            }
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
                <select
                    value={receiverId}
                    onChange={(e) => setReceiverId(e.target.value)}
                    required
                >
                    <option value="">Select user</option>

                    {users.map((user) => (
                        <option key={user.userId} value={user.userId}>
                            {user.login}
                        </option>
                    ))}
                </select>
            </div>

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
                    ? shoutout
                        ? "Saving..."
                        : "Adding..."
                    : shoutout
                    ? "Save changes"
                    : "Add shoutout"}
            </button>
        </form>
    );
}