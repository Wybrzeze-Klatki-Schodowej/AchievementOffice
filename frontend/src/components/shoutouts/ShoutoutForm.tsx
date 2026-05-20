// import { useState } from "react";
// import {
//     createShoutout,
//     updateShoutout,
//     type CreateShoutoutDto,
// } from "../../api/ShoutoutsApi";
// import type { Shoutout } from "../../types/shoutout";

// interface Props {
//     onShoutoutCreated: () => void;
//     shoutout?: Shoutout;
// }

// export default function ShoutoutForm({
//     onShoutoutCreated,
//     shoutout,
// }: Props) {
//     const [title, setTitle] = useState(
//         shoutout?.title ?? ""
//     );
//     const [description, setDescription] = useState(
//         shoutout?.description ?? ""
//     );
//     const [loading, setLoading] = useState(false);

//     const handleSubmit = async (
//         e: React.FormEvent<HTMLFormElement>
//     ) => {
//         e.preventDefault();

//         try {
//             setLoading(true);

//             if (shoutout) {
//                 await updateShoutout(
//                     shoutout.shoutoutId, 
//                     {
//                         title,
//                         description
//                     }
//                 );
//             } else {
//                 const dto: CreateShoutoutDto = {
//                     receiverId: "22222222-2222-2222-2222-222222222222",
//                     title,
//                     description
//                 };

//                 await createShoutout(dto);
//             }

//             onShoutoutCreated();

//             setTitle("");
//             setDescription("");
//         } catch (error) {
//             console.error(error);
//             alert("Operation failed");
//         } finally {
//             setLoading(false);
//         }
//     };

//     return (
//         <form onSubmit={handleSubmit}>
//             <h2>{shoutout ? "Edit shoutout" : "Add shoutout"}</h2>

//             <div>
//                 <input 
//                     type="text"
//                     placeholder="Title"
//                     value={title}
//                     onChange={(e) => setTitle(e.target.value)}
//                 />
//             </div>

//             <div>
//                 <textarea
//                     placeholder="Description"
//                     value={description}
//                     onChange={(e) => setDescription(e.target.value)}
//                     required
//                 />
//             </div>

//             <button type="submit" disabled={loading}>
//                 {loading ? shoutout ? "Saving..." : "Adding..." : shoutout ? "Save changes" : "Add shoutout"}
//             </button>
//         </form>
//     );
// }


// import { useEffect, useState } from "react";
// import {
//     createShoutout,
//     updateShoutout,
//     type CreateShoutoutDto,
// } from "../../api/ShoutoutsApi";
// import type { Shoutout } from "../../types/shoutout";

// interface User {
//     userId: string;
//     login: string;
// }

// interface Props {
//     onShoutoutCreated: () => void;
//     shoutout?: Shoutout;
// }

// export default function ShoutoutForm({
//     onShoutoutCreated,
//     shoutout,
// }: Props) {
//     const [title, setTitle] = useState(
//         shoutout?.title ?? ""
//     );

//     const [description, setDescription] = useState(
//         shoutout?.description ?? ""
//     );

//     const [receiverId, setReceiverId] = useState("");

//     const [users, setUsers] = useState<User[]>([]);

//     const [loading, setLoading] = useState(false);

//     useEffect(() => {
//         const fetchUsers = async () => {
//             try {
//                 const response = await fetch(
//                     "http://localhost:8080/api/users"
//                 );

//                 if (!response.ok) {
//                     throw new Error("Failed to fetch users");
//                 }

//                 const data = await response.json();

//                 setUsers(data);

//                 // opcjonalnie ustaw pierwszego usera jako default
//                 if (data.length > 0) {
//                     setReceiverId(data[0].userId);
//                 }
//             } catch (error) {
//                 console.log(receiverId);
//                 console.error(error);
//             }
//         };

//         fetchUsers();
//     }, []);

//     const handleSubmit = async (
//         e: React.FormEvent<HTMLFormElement>
//     ) => {
//         e.preventDefault();

//         try {
//             setLoading(true);

//             if (shoutout) {
//                 await updateShoutout(
//                     shoutout.shoutoutId,
//                     {
//                         title,
//                         description
//                     }
//                 );
//             } else {
//                 console.log(receiverId);
//                 const dto: CreateShoutoutDto = {
//                     receiverId,
//                     title,
//                     description
//                 };
//                 console.log(dto);

//                 await createShoutout(dto);
//             }

//             onShoutoutCreated();

//             setTitle("");
//             setDescription("");
//             setReceiverId("");
//         } catch (error) {
//             console.error(error);
//             alert("Operation failed");
//         } finally {
//             setLoading(false);
//         }
//     };

//     return (
//         <form onSubmit={handleSubmit}>
//             <h2>
//                 {shoutout
//                     ? "Edit shoutout"
//                     : "Add shoutout"}
//             </h2>

//             {!shoutout && (
//                 <div>
//                     <select
//                         value={receiverId}
//                         onChange={(e) =>
//                             setReceiverId(e.target.value)
//                         }
//                         required
//                     >
//                         <option value="">
//                             Select user
//                         </option>

//                         {users.map((user) => (
//                             <option
//                                 key={user.userId}
//                                 value={user.userId}
//                             >
//                                 {user.login}
//                             </option>
//                         ))}
//                     </select>
//                 </div>
//             )}

//             <div>
//                 <input
//                     type="text"
//                     placeholder="Title"
//                     value={title}
//                     onChange={(e) =>
//                         setTitle(e.target.value)
//                     }
//                 />
//             </div>

//             <div>
//                 <textarea
//                     placeholder="Description"
//                     value={description}
//                     onChange={(e) =>
//                         setDescription(
//                             e.target.value
//                         )
//                     }
//                     required
//                 />
//             </div>

//             <button
//                 type="submit"
//                 disabled={loading}
//             >
//                 {loading
//                     ? shoutout
//                         ? "Saving..."
//                         : "Adding..."
//                     : shoutout
//                     ? "Save changes"
//                     : "Add shoutout"}
//             </button>
//         </form>
//     );
// }

// import { useEffect, useState } from "react";
// import {
//     createShoutout,
//     updateShoutout,
//     type CreateShoutoutDto,
// } from "../../api/ShoutoutsApi";
// import type { Shoutout } from "../../types/shoutout";

// interface User {
//     userId: string;
//     login: string;
// }

// interface Props {
//     onShoutoutCreated: () => void;
//     shoutout?: Shoutout;
// }

// export default function ShoutoutForm({
//     onShoutoutCreated,
//     shoutout,
// }: Props) {
//     const [title, setTitle] = useState(
//         shoutout?.title ?? ""
//     );

//     const [description, setDescription] = useState(
//         shoutout?.description ?? ""
//     );

//     const [receiverId, setReceiverId] = useState("");

//     const [users, setUsers] = useState<User[]>([]);

//     const [loading, setLoading] = useState(false);

//     useEffect(() => {
//         const fetchUsers = async () => {
//             try {
//                 const token = localStorage.getItem("jwtToken");

//                 const response = await fetch(
//                     "http://localhost:8080/api/users",
//                     {
//                         headers: {
//                             Authorization: `Bearer ${token}`,
//                         },
//                     }
//                 );

//                 if (!response.ok) {
//                     throw new Error(
//                         "Failed to fetch users"
//                     );
//                 }

//                 const data: User[] =
//                     await response.json();

//                 setUsers(data);

//                 // ustaw pierwszego usera jako default
//                 if (data.length > 0) {
//                     setReceiverId(
//                         data[0].userId
//                     );
//                 }
//             } catch (error) {
//                 console.error(
//                     "FETCH USERS ERROR:",
//                     error
//                 );
//             }
//         };

//         fetchUsers();
//     }, []);

//     const handleSubmit = async (
//         e: React.FormEvent<HTMLFormElement>
//     ) => {
//         e.preventDefault();

//         try {
//             setLoading(true);

//             const token = localStorage.getItem("jwtToken");
//             console.log(localStorage);

//             if (!token) {
//                 throw new Error(
//                     "User is not authenticated"
//                 );
//             }

//             if (shoutout) {
//                 await updateShoutout(
//                     shoutout.shoutoutId,
//                     {
//                         title,
//                         description,
//                     },
//                     token
//                 );
//             } else {
//                 const dto: CreateShoutoutDto =
//                     {
//                         receiverId,
//                         title,
//                         description,
//                     };

//                 console.log(
//                     "Creating shoutout:",
//                     dto
//                 );

//                 await createShoutout(
//                     dto,
//                     token
//                 );
//             }

//             onShoutoutCreated();

//             setTitle("");
//             setDescription("");

//             if (users.length > 0) {
//                 setReceiverId(
//                     users[0].userId
//                 );
//             }
//         } catch (error) {
//             console.error(
//                 "SHOUTOUT ERROR:",
//                 error
//             );

//             if (error instanceof Error) {
//                 alert(error.message);
//             } else {
//                 alert("Operation failed");
//             }
//         } finally {
//             setLoading(false);
//         }
//     };

//     return (
//         <form onSubmit={handleSubmit}>
//             <h2>
//                 {shoutout
//                     ? "Edit shoutout"
//                     : "Add shoutout"}
//             </h2>

//             {!shoutout && (
//                 <div>
//                     <select
//                         value={receiverId}
//                         onChange={(e) =>
//                             setReceiverId(
//                                 e.target.value
//                             )
//                         }
//                         required
//                     >
//                         <option value="">
//                             Select user
//                         </option>

//                         {users.map((user) => (
//                             <option
//                                 key={user.userId}
//                                 value={
//                                     user.userId
//                                 }
//                             >
//                                 {user.login}
//                             </option>
//                         ))}
//                     </select>
//                 </div>
//             )}

//             <div>
//                 <input
//                     type="text"
//                     placeholder="Title"
//                     value={title}
//                     onChange={(e) =>
//                         setTitle(
//                             e.target.value
//                         )
//                     }
//                     required
//                 />
//             </div>

//             <div>
//                 <textarea
//                     placeholder="Description"
//                     value={description}
//                     onChange={(e) =>
//                         setDescription(
//                             e.target.value
//                         )
//                     }
//                     required
//                 />
//             </div>

//             <button
//                 type="submit"
//                 disabled={
//                     loading ||
//                     (!shoutout &&
//                         !receiverId)
//                 }
//             >
//                 {loading
//                     ? shoutout
//                         ? "Saving..."
//                         : "Adding..."
//                     : shoutout
//                     ? "Save changes"
//                     : "Add shoutout"}
//             </button>
//         </form>
//     );
// }

import { useEffect, useState } from "react";
import {
    createShoutout,
    updateShoutout,
    type CreateShoutoutDto,
} from "../../api/ShoutoutsApi";
import type { Shoutout } from "../../types/shoutout";

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

                setUsers(data);

                if (data.length > 0) {
                    setReceiverId(data[0].userId);
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