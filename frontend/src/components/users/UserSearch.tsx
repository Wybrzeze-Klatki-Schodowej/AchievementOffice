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
        <input 
            className="user-search"
            type="text"
            placeholder="Search users..."
            value={value}
            onChange={(e) =>
                onChange(e.target.value)
            }
        />
    );
}