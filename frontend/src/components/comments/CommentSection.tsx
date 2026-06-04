import { useEffect, useState } from "react";
import { createComment, getComments } from "../../api/CommentApi";
import type { Comment } from "../../types/comment";

interface Props {
    profileUserId: string;
}

export default function CommentSection({
    profileUserId
}: Props) {

    const [comments, setComments] = useState<Comment[]>([]);

    const [content, setContent] = useState("");

    async function loadComments() {
        try {
            const data = await getComments(profileUserId);

            setComments(data);
        }
        catch (err) {
            console.error(err);
        }
    }

    async function handleAddComment() {
        if (!content.trim()) {
            return;
        }

        try {
            await createComment(
                profileUserId,
                {
                    content
                }
            );

            setContent("");

            await loadComments();
        }
        catch (err) {
            console.error(err);
        }
    }

    useEffect(() => {
        loadComments();
    }, [profileUserId]);

    return (
        <div className="profile-card">
            <h2>Comments</h2>

            <textarea 
                value={content}
                onChange={(e) =>
                    setContent(e.target.value)
                }
                rows={4}
                placeholder="Write a comment..."
            />

            <button 
                onClick={handleAddComment}
            >
                Add Comment
            </button>

            {comments.map(comment => (
                <div 
                    key={comment.id}
                    className="comment-card"
                >
                    <b>
                        {comment.authorFirstName} 
                        {" "}
                        {comment.authorLastName}
                    </b>

                    <div>
                        @{comment.authorLogin}
                    </div>

                    <div>
                        {new Date(
                            comment.createdAt
                            ).toLocaleString()}
                    </div>

                    <p>
                        {comment.content}
                    </p>
                </div>
            ))}
        </div>
    );
}