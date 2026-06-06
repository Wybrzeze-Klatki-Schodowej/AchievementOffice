import "./UserSearch.css";

interface Props {
    value: string;
    onChange: (value: string) => void;
}

export default function UserSearch({
    value,
    onChange
}: Props) {
    return (
        <div className="user-search-container">
            <label className="user-search-label">
                Search
            </label>

            <input 
                className="user-search"
                type="text"
                placeholder="Search users..."
                value={value}
                onChange={(e) =>
                    onChange(e.target.value)
                }
            />
        </div>
    );
}