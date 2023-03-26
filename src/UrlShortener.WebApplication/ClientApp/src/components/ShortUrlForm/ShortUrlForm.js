import React, { useState, useRef } from 'react';
import { createShortUrl } from '../../api';
import { FiCopy } from 'react-icons/fi';
import styles from './ShortUrlForm.module.css';

const ShortUrlForm = ({ onShortUrlCreated }) => {
  const [originalUrl, setOriginalUrl] = useState('');
  const [loading, setLoading] = useState(false);
  const [shortUrl, setShortUrl] = useState('');
  const [showCopyTooltip, setShowCopyTooltip] = useState(false);
  const copyInputRef = useRef(null);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setLoading(true);
    try {
      const response = await createShortUrl(originalUrl);
      const createdShortUrl = response.value.url;
      setShortUrl(createdShortUrl);
      onShortUrlCreated(shortUrl);
    } catch (error) {
      console.error('Error creating short URL:', error);
    } finally {
      setLoading(false);
    }
  };

  const copyToClipboard = async (text) => {
    try {
      await navigator.clipboard.writeText(text);
      setShowCopyTooltip(true);
      setTimeout(() => setShowCopyTooltip(false), 2000);
    } catch (err) {
      console.error('Failed to copy text: ', err);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Create a Short URL</h2>
      <label htmlFor="originalUrl">Original URL:</label>
      <div className={styles.inputContainer}>
        <input
          id="originalUrl"
          type="text"
          value={originalUrl}
          placeholder="Enter URL"
          className={styles.urlInput}
          onChange={(event) => setOriginalUrl(event.target.value)}
        />
      </div>
      <button className={styles.shortenButton} type="submit" disabled={loading}>
        {loading ? 'Creating...' : 'Create Short URL'}
      </button>
      {shortUrl && (
        <div>
          <label htmlFor="createdUrl">Created URL:</label>
          <div className={styles.inputContainer}>
            <input
              id="createdUrl"
              type="text"
              value={shortUrl}
              readOnly
              className={styles.urlInput}
              onClick={() => copyToClipboard(shortUrl)}
              ref={copyInputRef}
            />
            <FiCopy className={styles.copyIcon} onClick={() => copyToClipboard(shortUrl)} />
          </div>
          {showCopyTooltip && (
                  <span className={styles.copyTooltip}>Copied to clipboard!</span>
                  )}
        </div>
        )}
    </form>
    );
};

export default ShortUrlForm;
