import { useState } from 'react';
import axios from 'axios';

function LikeButton({ userId, postId }) {
  const [isLiked, setIsLiked] = useState(false);

  const toggleLike = async () => {
    try {
      const response = await axios.post('/api/like/toggle', {
        userId,
        postId
      });
      setIsLiked(response.data.isLiked);
    } catch (error) {
      console.error('いいね失敗', error);
    }
  };

  return (
    <button onClick={toggleLike} style={{ background: 'none', border: 'none', cursor: 'pointer' }}>
      <span style={{ color: isLiked ? 'red' : 'gray', fontSize: '24px' }}>
        {isLiked ? '❤️' : '🤍'}
      </span>
    </button>
  );
}

export default LikeButton;
