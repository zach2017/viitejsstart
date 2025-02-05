import { useNavigate } from 'react-router-dom';
import useAuthStore from '../auth/authStore';

const PrivatePage = () => {
  const { isAuthenticated } = useAuthStore();
  const navigate = useNavigate();

  if (!isAuthenticated) {
    return (
      <div>
        <h1>Access Denied</h1>
        <p>You must log in to view this page.</p>
        <button onClick={() => navigate('/')}>Go to Home</button>
      </div>
    );
  }

  return (
    <div>
      <h1>Private Page</h1>
      <p>Only logged-in users can see this.</p>
    </div>
  );
};

export default PrivatePage;
