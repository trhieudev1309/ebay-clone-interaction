// Real data fed from the server
let chats = [];
if (typeof initialChats !== 'undefined') {
    // Map backend DTOs to the expected structure (handling both PascalCase and camelCase safely)
    chats = initialChats.map(c => {
        const sId = c.SenderId !== undefined ? c.SenderId : c.senderId;
        const rId = c.ReceiverId !== undefined ? c.ReceiverId : c.receiverId;
        return {
            id: (sId === currentUserId) ? rId : sId,
            name: c.Name || c.name,
            preview: c.LastMessage || c.lastMessage,
            online: false, 
            avatar: c.Avatar || c.avatar || 'https://i.pravatar.cc/150',
            messages: [] 
        };
    });
}

let currentChat = null;
const base = window.location.origin;

// Setup SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${base}/chatHub`)
    .withAutomaticReconnect()
    .build();

// Keep-alive settings
connection.serverTimeoutInMilliseconds = 60000;
connection.keepAliveIntervalInMilliseconds = 15000;

connection.start().then(async () => {
    console.log("BoxChat SignalR Connected!");
    try {
        await connection.invoke("ConnectUser", currentUserId);
    } catch {}
}).catch(err => {
    console.error("SignalR Connection Error: ", err);
});

connection.onreconnected(async (id) => {
    try {
        await connection.invoke("ConnectUser", currentUserId);
    } catch {}
});

// Receive live messages
connection.on("ReceiveMessage", async (messageData) => {
    const isSentByMe = parseInt(messageData.senderId) === parseInt(currentUserId);
    const senderOrReceiverId = isSentByMe ? parseInt(messageData.receiverId) : parseInt(messageData.senderId);

    // Immediately stop typing indicator if receiving a message
    if (!isSentByMe) hideTypingIndicator();

    // If the message belongs to the active chat
    if (currentChat && (parseInt(messageData.senderId) === parseInt(currentChat.id) || parseInt(messageData.receiverId) === parseInt(currentChat.id))) {
        appendSingleMessage(messageData); // Use optimized append instead of clearing innerHTML
    }
    
    // Update the sidebar preview
    let chatIndex = chats.findIndex(c => c.id === senderOrReceiverId);
    let chatItem;

    if (chatIndex !== -1) {
        chatItem = chats.splice(chatIndex, 1)[0]; // Remove from current position
        chatItem.preview = messageData.content;
    } else {
        // It's a completely new conversation! Push it to the top.
        chatItem = {
            id: senderOrReceiverId,
            name: isSentByMe ? "Me" : messageData.senderName,
            preview: messageData.content,
            online: false,
            avatar: 'https://i.pravatar.cc/150',
            messages: []
        };
    }
    
    chats.unshift(chatItem); // Add to beginning (Realtime UX)
    initChatList(); // re-render sidebar
});

// Typing indicator Real-time UX
let typingTimer = null;
connection.on("UserIsTyping", (senderId) => {
    if (currentChat && senderId === currentChat.id) {
        showTypingIndicator();
        clearTimeout(typingTimer);
        // Hide after 2 seconds of inactivity
        typingTimer = setTimeout(hideTypingIndicator, 2000);
    }
});

function showTypingIndicator() {
    const indicator = document.getElementById('typingIndicator');
    if (indicator) {
        indicator.classList.remove('d-none');
        scrollToBottom(true);
    }
}

function hideTypingIndicator() {
    const indicator = document.getElementById('typingIndicator');
    if (indicator) indicator.classList.add('d-none');
}

// Input event listener for broadcasting "typing" status
let isTyping = false;
let emitTypingTimer = null;
const messageInput = document.getElementById('messageInput');
if (messageInput) {
    messageInput.addEventListener('input', () => {
        if (!currentChat) return;

        if (!isTyping) {
            isTyping = true;
            // Broadcast typing via SignalR hub
            connection.invoke("UserTyping", parseInt(currentUserId), parseInt(currentChat.id)).catch(err => console.error(err));
        }

        clearTimeout(emitTypingTimer);
        emitTypingTimer = setTimeout(() => {
            isTyping = false;
        }, 1500); // 1.5 seconds cooldown
    });
}

// Initialize chat list
function initChatList() {
    const chatList = document.getElementById('chatList');
    chatList.innerHTML = '';

    chats.forEach(chat => {
        const chatItem = document.createElement('div');
        chatItem.className = 'chat-item';
        // Add bolding to the top item for Real-time UX
        chatItem.innerHTML = `
            <img src="${chat.avatar}" alt="${chat.name}" class="chat-avatar">
            <div class="chat-info">
                <div class="chat-name" style="${chats.length > 0 && chat.id === chats[0].id ? 'font-weight: bold;' : ''}">${chat.name}</div>
                <div class="chat-preview" style="${chats.length > 0 && chat.id === chats[0].id ? 'color: #333;' : ''}">${escapeHtml(chat.preview || '...')}</div>
            </div>
        `;
        chatItem.addEventListener('click', () => selectChat(chat.id));
        chatList.appendChild(chatItem);
    });

    // Keep active state visually consistent
    if (currentChat) {
        document.querySelectorAll('.chat-item').forEach((item, index) => {
            item.classList.toggle('active', chats[index].id === currentChat.id);
        });
    }
}

// Select chat
async function selectChat(chatId) {
    if (currentChat && currentChat.id === chatId) return; // Ignore if clicking already active chat
    
    currentChat = chats.find(c => c.id === chatId);
    hideTypingIndicator();

    // Update sidebar active state
    document.querySelectorAll('.chat-item').forEach((item, index) => {
        item.classList.toggle('active', chats[index].id === chatId);
    });

    // Update header
    document.getElementById('chatName').textContent = currentChat.name;
    document.getElementById('chatStatus').textContent = currentChat.online ? 'Active now' : 'Offline';

    // Load actual message history from API
    await loadMessages();
}

async function loadMessages() {
    try {
        const res = await fetch(`${base}/Chat/GetMessages?senderId=${currentUserId}&receiverId=${currentChat.id}`);
        const messages = await res.json();
        
        const messagesContainer = document.getElementById('messagesContainer');
        messagesContainer.innerHTML = '';
        
        // Use DocumentFragment for performance optimization (No lag on huge histories)
        const fragment = document.createDocumentFragment();
        
        messages.forEach(messageData => {
            const el = createMessageElement(messageData);
            fragment.appendChild(el);
        });
        
        messagesContainer.appendChild(fragment);
        scrollToBottom(false); // Initial load jumps instantly to bottom
    } catch (e) {
        console.error("Error loading messages:", e);
    }
}

function createMessageElement(messageData) {
    const isMe = parseInt(messageData.senderId) === parseInt(currentUserId);
    const msgType = isMe ? "sent" : "received";
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${msgType}`;
    
    let timeParts = (messageData.sentAt || "").split(" ");
    let timeStr = timeParts[0] || messageData.sentAt || "";
    let dateStr = timeParts[1] || "";

    messageDiv.innerHTML = `
        <div class="message-bubble">
            <div>${escapeHtml(messageData.content)}</div>
            <div class="message-time">${timeStr} ${dateStr ? '| ' + dateStr : ''}</div>
        </div>
    `;
    return messageDiv;
}

// Appends single message directly to DOM (fastest) instead of re-rendering everything
function appendSingleMessage(messageData) {
    const messagesContainer = document.getElementById('messagesContainer');
    const el = createMessageElement(messageData);
    messagesContainer.appendChild(el);
    
    scrollToBottom(true); // Smooth scroll for new message
}

// Ensure old backwards compatibility
function displayMessage(messageData) {
    appendSingleMessage(messageData);
}

function scrollToBottom(smooth = false) {
    const messagesContainer = document.getElementById('messagesContainer');
    if (!messagesContainer) return;
    
    messagesContainer.scrollTo({
        top: messagesContainer.scrollHeight,
        behavior: smooth ? 'smooth' : 'auto'
    });
}

// Send message
async function sendMessage() {
    const input = document.getElementById('messageInput');
    const text = input.value.trim();

    if (!text || !currentChat) return;

    try {
        input.value = '';
        input.focus();
        
        // Hide typing locally and tell others we stopped
        isTyping = false;
        clearTimeout(emitTypingTimer);
        
        await connection.invoke("SendMessage", parseInt(currentUserId), parseInt(currentChat.id), text);
    } catch (e) {
        console.error("Error sending message:", e);
        alert("Failed to send message. Error: " + (e.message || JSON.stringify(e)));
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text ?? "";
    return div.innerHTML;
}

// Event listeners
document.getElementById('sendBtn').addEventListener('click', sendMessage);
document.getElementById('messageInput').addEventListener('keypress', (e) => {
    if (e.key === 'Enter') sendMessage();
});

// Initialize on page load
initChatList();
if (chats.length > 0) {
    selectChat(chats[0].id);
}