import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import RegisterForm from '../components/register/RegisterForm';
import { register, type RegisterDTO } from '../api/RegisterApi';

export default function RegisterPage() {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleRegister = async (data: RegisterDTO) => {
        setIsLoading(true);
        setError(null);
        try {
            await register(data);
            
            navigate('/login');
        } catch (err: any) {
            setError(err.message || 'Error connecting to server');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
            <RegisterForm isLoading={isLoading} onSubmit={handleRegister} error={error} />
        </div>
    );
}