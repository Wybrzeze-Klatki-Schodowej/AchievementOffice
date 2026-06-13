import React, { useState } from 'react';
import type { RegisterDTO } from '../../api/RegisterApi';
import '../login/LoginForm.css';
import { Link } from 'react-router-dom';

interface RegisterFormProps {
    isLoading: boolean;
    onSubmit: (data: RegisterDTO) => void;
    error: string | null;
}

export default function RegisterForm({ isLoading, onSubmit, error }: RegisterFormProps) {
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [firstname, setFirstname] = useState('');
    const [lastname, setLastname] = useState('');
    const [jobTitle, setJobTitle] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit({ 
            Email: email, 
            Username: username, 
            Password: password, 
            Firstname: firstname, 
            Lastname: lastname, 
            JobTitle: jobTitle,
            RoleName: 'User'
        });
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Register</h2>
            <input type="text" placeholder='Email' value={email} onChange={(e) => setEmail(e.target.value)} />
            <input type="text" placeholder='Username' value={username} onChange={(e) => setUsername(e.target.value)} />
            <input type="password" placeholder='Password (min. 8 characters)' value={password} onChange={(e) => setPassword(e.target.value)} />
            <input type="text" placeholder='First Name' value={firstname} onChange={(e) => setFirstname(e.target.value)} />
            <input type="text" placeholder='Last Name' value={lastname} onChange={(e) => setLastname(e.target.value)} />
            <input type="text" placeholder='Job Title' value={jobTitle} onChange={(e) => setJobTitle(e.target.value)} />
            
            {error && (
                <pre 
                    className="error-message"
                    style={{
                        color: 'red', 
                        margin: '1rem 0',
                        whiteSpace: 'pre-wrap',
                        fontFamily: 'inherit'
                    }}
                >
                    {error}
                </pre>
            )}
            
            <button className="login-button" type="submit" disabled={isLoading}>
                {isLoading ? 'Loading...' : 'Register'}
            </button>

            <div style={{marginTop: '1rem', fontSize: '0.9rem'}}>
                Already have an account? <Link to="/login">Login</Link>
            </div>
        </form>
    );
}