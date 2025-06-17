import React from 'react';
import { createRoot } from 'react-dom/client';
import LikeButton from './components/LikeButton';

document.querySelectorAll('[id^="like-button-root-"]').forEach(container => {
  const userId = parseInt(container.dataset.userId);
  const postId = parseInt(container.dataset.postId);

  const root = createRoot(container);
  root.render(<LikeButton userId={userId} postId={postId} />);
});
